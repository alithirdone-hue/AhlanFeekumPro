using AhlanFeekumPro.Governorates;
using AhlanFeekumPro.PropertyTypes;
using AhlanFeekumPro.PropertyFeatures;
using AhlanFeekumPro.PropertyFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using AhlanFeekumPro.EntityFrameworkCore;

namespace AhlanFeekumPro.SiteProperties
{
    public abstract class EfCoreSitePropertyRepositoryBase : EfCoreRepository<AhlanFeekumProDbContext, SiteProperty, Guid>
    {
        public EfCoreSitePropertyRepositoryBase(IDbContextProvider<AhlanFeekumProDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        string? propertyTitle = null,
            string? hotelName = null,
            int? bedroomsMin = null,
            int? bedroomsMax = null,
            int? bathroomsMin = null,
            int? bathroomsMax = null,
            int? numberOfBedMin = null,
            int? numberOfBedMax = null,
            int? floorMin = null,
            int? floorMax = null,
            int? maximumNumberOfGuestMin = null,
            int? maximumNumberOfGuestMax = null,
            int? livingroomsMin = null,
            int? livingroomsMax = null,
            string? propertyDescription = null,
            string? hourseRules = null,
            string? importantInformation = null,
            string? address = null,
            string? streetAndBuildingNumber = null,
            string? landMark = null,
            int? pricePerNightMin = null,
            int? pricePerNightMax = null,
            bool? isActive = null,
            Guid? propertyTypeId = null,
            Guid? governorateId = null,
            Guid? propertyFeatureId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, propertyTitle, hotelName, bedroomsMin, bedroomsMax, bathroomsMin, bathroomsMax, numberOfBedMin, numberOfBedMax, floorMin, floorMax, maximumNumberOfGuestMin, maximumNumberOfGuestMax, livingroomsMin, livingroomsMax, propertyDescription, hourseRules, importantInformation, address, streetAndBuildingNumber, landMark, pricePerNightMin, pricePerNightMax, isActive, propertyTypeId, governorateId, propertyFeatureId);

            var ids = query.Select(x => x.SiteProperty.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<SitePropertyWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id).Include(x => x.PropertyFeatures)
                .Select(siteProperty => new SitePropertyWithNavigationProperties
                {
                    SiteProperty = siteProperty,
                    PropertyType = dbContext.Set<PropertyType>().FirstOrDefault(c => c.Id == siteProperty.PropertyTypeId),
                    Governorate = dbContext.Set<Governorate>().FirstOrDefault(c => c.Id == siteProperty.GovernorateId),
                    PropertyFeatures = (from sitePropertyPropertyFeatures in siteProperty.PropertyFeatures
                                        join _propertyFeature in dbContext.Set<PropertyFeature>() on sitePropertyPropertyFeatures.PropertyFeatureId equals _propertyFeature.Id
                                        select _propertyFeature).ToList()
                }).FirstOrDefault();
        }

        public virtual async Task<List<SitePropertyWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? propertyTitle = null,
            string? hotelName = null,
            int? bedroomsMin = null,
            int? bedroomsMax = null,
            int? bathroomsMin = null,
            int? bathroomsMax = null,
            int? numberOfBedMin = null,
            int? numberOfBedMax = null,
            int? floorMin = null,
            int? floorMax = null,
            int? maximumNumberOfGuestMin = null,
            int? maximumNumberOfGuestMax = null,
            int? livingroomsMin = null,
            int? livingroomsMax = null,
            string? propertyDescription = null,
            string? hourseRules = null,
            string? importantInformation = null,
            string? address = null,
            string? streetAndBuildingNumber = null,
            string? landMark = null,
            int? pricePerNightMin = null,
            int? pricePerNightMax = null,
            bool? isActive = null,
            Guid? propertyTypeId = null,
            Guid? governorateId = null,
            Guid? propertyFeatureId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, propertyTitle, hotelName, bedroomsMin, bedroomsMax, bathroomsMin, bathroomsMax, numberOfBedMin, numberOfBedMax, floorMin, floorMax, maximumNumberOfGuestMin, maximumNumberOfGuestMax, livingroomsMin, livingroomsMax, propertyDescription, hourseRules, importantInformation, address, streetAndBuildingNumber, landMark, pricePerNightMin, pricePerNightMax, isActive, propertyTypeId, governorateId, propertyFeatureId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? SitePropertyConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<SitePropertyWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from siteProperty in (await GetDbSetAsync())
                   join propertyType in (await GetDbContextAsync()).Set<PropertyType>() on siteProperty.PropertyTypeId equals propertyType.Id into propertyTypes
                   from propertyType in propertyTypes.DefaultIfEmpty()
                   join governorate in (await GetDbContextAsync()).Set<Governorate>() on siteProperty.GovernorateId equals governorate.Id into governorates
                   from governorate in governorates.DefaultIfEmpty()
                   select new SitePropertyWithNavigationProperties
                   {
                       SiteProperty = siteProperty,
                       PropertyType = propertyType,
                       Governorate = governorate,
                       PropertyFeatures = new List<PropertyFeature>()
                   };
        }

        protected virtual IQueryable<SitePropertyWithNavigationProperties> ApplyFilter(
            IQueryable<SitePropertyWithNavigationProperties> query,
            string? filterText,
            string? propertyTitle = null,
            string? hotelName = null,
            int? bedroomsMin = null,
            int? bedroomsMax = null,
            int? bathroomsMin = null,
            int? bathroomsMax = null,
            int? numberOfBedMin = null,
            int? numberOfBedMax = null,
            int? floorMin = null,
            int? floorMax = null,
            int? maximumNumberOfGuestMin = null,
            int? maximumNumberOfGuestMax = null,
            int? livingroomsMin = null,
            int? livingroomsMax = null,
            string? propertyDescription = null,
            string? hourseRules = null,
            string? importantInformation = null,
            string? address = null,
            string? streetAndBuildingNumber = null,
            string? landMark = null,
            int? pricePerNightMin = null,
            int? pricePerNightMax = null,
            bool? isActive = null,
            Guid? propertyTypeId = null,
            Guid? governorateId = null,
            Guid? propertyFeatureId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.SiteProperty.PropertyTitle!.Contains(filterText!) || e.SiteProperty.HotelName!.Contains(filterText!) || e.SiteProperty.PropertyDescription!.Contains(filterText!) || e.SiteProperty.HourseRules!.Contains(filterText!) || e.SiteProperty.ImportantInformation!.Contains(filterText!) || e.SiteProperty.Address!.Contains(filterText!) || e.SiteProperty.StreetAndBuildingNumber!.Contains(filterText!) || e.SiteProperty.LandMark!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(propertyTitle), e => e.SiteProperty.PropertyTitle.Contains(propertyTitle))
                    .WhereIf(!string.IsNullOrWhiteSpace(hotelName), e => e.SiteProperty.HotelName.Contains(hotelName))
                    .WhereIf(bedroomsMin.HasValue, e => e.SiteProperty.Bedrooms >= bedroomsMin!.Value)
                    .WhereIf(bedroomsMax.HasValue, e => e.SiteProperty.Bedrooms <= bedroomsMax!.Value)
                    .WhereIf(bathroomsMin.HasValue, e => e.SiteProperty.Bathrooms >= bathroomsMin!.Value)
                    .WhereIf(bathroomsMax.HasValue, e => e.SiteProperty.Bathrooms <= bathroomsMax!.Value)
                    .WhereIf(numberOfBedMin.HasValue, e => e.SiteProperty.NumberOfBed >= numberOfBedMin!.Value)
                    .WhereIf(numberOfBedMax.HasValue, e => e.SiteProperty.NumberOfBed <= numberOfBedMax!.Value)
                    .WhereIf(floorMin.HasValue, e => e.SiteProperty.Floor >= floorMin!.Value)
                    .WhereIf(floorMax.HasValue, e => e.SiteProperty.Floor <= floorMax!.Value)
                    .WhereIf(maximumNumberOfGuestMin.HasValue, e => e.SiteProperty.MaximumNumberOfGuest >= maximumNumberOfGuestMin!.Value)
                    .WhereIf(maximumNumberOfGuestMax.HasValue, e => e.SiteProperty.MaximumNumberOfGuest <= maximumNumberOfGuestMax!.Value)
                    .WhereIf(livingroomsMin.HasValue, e => e.SiteProperty.Livingrooms >= livingroomsMin!.Value)
                    .WhereIf(livingroomsMax.HasValue, e => e.SiteProperty.Livingrooms <= livingroomsMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(propertyDescription), e => e.SiteProperty.PropertyDescription.Contains(propertyDescription))
                    .WhereIf(!string.IsNullOrWhiteSpace(hourseRules), e => e.SiteProperty.HourseRules.Contains(hourseRules))
                    .WhereIf(!string.IsNullOrWhiteSpace(importantInformation), e => e.SiteProperty.ImportantInformation.Contains(importantInformation))
                    .WhereIf(!string.IsNullOrWhiteSpace(address), e => e.SiteProperty.Address.Contains(address))
                    .WhereIf(!string.IsNullOrWhiteSpace(streetAndBuildingNumber), e => e.SiteProperty.StreetAndBuildingNumber.Contains(streetAndBuildingNumber))
                    .WhereIf(!string.IsNullOrWhiteSpace(landMark), e => e.SiteProperty.LandMark.Contains(landMark))
                    .WhereIf(pricePerNightMin.HasValue, e => e.SiteProperty.PricePerNight >= pricePerNightMin!.Value)
                    .WhereIf(pricePerNightMax.HasValue, e => e.SiteProperty.PricePerNight <= pricePerNightMax!.Value)
                    .WhereIf(isActive.HasValue, e => e.SiteProperty.IsActive == isActive)
                    .WhereIf(propertyTypeId != null && propertyTypeId != Guid.Empty, e => e.PropertyType != null && e.PropertyType.Id == propertyTypeId)
                    .WhereIf(governorateId != null && governorateId != Guid.Empty, e => e.Governorate != null && e.Governorate.Id == governorateId)
                    .WhereIf(propertyFeatureId != null && propertyFeatureId != Guid.Empty, e => e.SiteProperty.PropertyFeatures.Any(x => x.PropertyFeatureId == propertyFeatureId));
        }

        public virtual async Task<List<SiteProperty>> GetListAsync(
            string? filterText = null,
            string? propertyTitle = null,
            string? hotelName = null,
            int? bedroomsMin = null,
            int? bedroomsMax = null,
            int? bathroomsMin = null,
            int? bathroomsMax = null,
            int? numberOfBedMin = null,
            int? numberOfBedMax = null,
            int? floorMin = null,
            int? floorMax = null,
            int? maximumNumberOfGuestMin = null,
            int? maximumNumberOfGuestMax = null,
            int? livingroomsMin = null,
            int? livingroomsMax = null,
            string? propertyDescription = null,
            string? hourseRules = null,
            string? importantInformation = null,
            string? address = null,
            string? streetAndBuildingNumber = null,
            string? landMark = null,
            int? pricePerNightMin = null,
            int? pricePerNightMax = null,
            bool? isActive = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, propertyTitle, hotelName, bedroomsMin, bedroomsMax, bathroomsMin, bathroomsMax, numberOfBedMin, numberOfBedMax, floorMin, floorMax, maximumNumberOfGuestMin, maximumNumberOfGuestMax, livingroomsMin, livingroomsMax, propertyDescription, hourseRules, importantInformation, address, streetAndBuildingNumber, landMark, pricePerNightMin, pricePerNightMax, isActive);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? SitePropertyConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? propertyTitle = null,
            string? hotelName = null,
            int? bedroomsMin = null,
            int? bedroomsMax = null,
            int? bathroomsMin = null,
            int? bathroomsMax = null,
            int? numberOfBedMin = null,
            int? numberOfBedMax = null,
            int? floorMin = null,
            int? floorMax = null,
            int? maximumNumberOfGuestMin = null,
            int? maximumNumberOfGuestMax = null,
            int? livingroomsMin = null,
            int? livingroomsMax = null,
            string? propertyDescription = null,
            string? hourseRules = null,
            string? importantInformation = null,
            string? address = null,
            string? streetAndBuildingNumber = null,
            string? landMark = null,
            int? pricePerNightMin = null,
            int? pricePerNightMax = null,
            bool? isActive = null,
            Guid? propertyTypeId = null,
            Guid? governorateId = null,
            Guid? propertyFeatureId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, propertyTitle, hotelName, bedroomsMin, bedroomsMax, bathroomsMin, bathroomsMax, numberOfBedMin, numberOfBedMax, floorMin, floorMax, maximumNumberOfGuestMin, maximumNumberOfGuestMax, livingroomsMin, livingroomsMax, propertyDescription, hourseRules, importantInformation, address, streetAndBuildingNumber, landMark, pricePerNightMin, pricePerNightMax, isActive, propertyTypeId, governorateId, propertyFeatureId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<SiteProperty> ApplyFilter(
            IQueryable<SiteProperty> query,
            string? filterText = null,
            string? propertyTitle = null,
            string? hotelName = null,
            int? bedroomsMin = null,
            int? bedroomsMax = null,
            int? bathroomsMin = null,
            int? bathroomsMax = null,
            int? numberOfBedMin = null,
            int? numberOfBedMax = null,
            int? floorMin = null,
            int? floorMax = null,
            int? maximumNumberOfGuestMin = null,
            int? maximumNumberOfGuestMax = null,
            int? livingroomsMin = null,
            int? livingroomsMax = null,
            string? propertyDescription = null,
            string? hourseRules = null,
            string? importantInformation = null,
            string? address = null,
            string? streetAndBuildingNumber = null,
            string? landMark = null,
            int? pricePerNightMin = null,
            int? pricePerNightMax = null,
            bool? isActive = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.PropertyTitle!.Contains(filterText!) || e.HotelName!.Contains(filterText!) || e.PropertyDescription!.Contains(filterText!) || e.HourseRules!.Contains(filterText!) || e.ImportantInformation!.Contains(filterText!) || e.Address!.Contains(filterText!) || e.StreetAndBuildingNumber!.Contains(filterText!) || e.LandMark!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(propertyTitle), e => e.PropertyTitle.Contains(propertyTitle))
                    .WhereIf(!string.IsNullOrWhiteSpace(hotelName), e => e.HotelName.Contains(hotelName))
                    .WhereIf(bedroomsMin.HasValue, e => e.Bedrooms >= bedroomsMin!.Value)
                    .WhereIf(bedroomsMax.HasValue, e => e.Bedrooms <= bedroomsMax!.Value)
                    .WhereIf(bathroomsMin.HasValue, e => e.Bathrooms >= bathroomsMin!.Value)
                    .WhereIf(bathroomsMax.HasValue, e => e.Bathrooms <= bathroomsMax!.Value)
                    .WhereIf(numberOfBedMin.HasValue, e => e.NumberOfBed >= numberOfBedMin!.Value)
                    .WhereIf(numberOfBedMax.HasValue, e => e.NumberOfBed <= numberOfBedMax!.Value)
                    .WhereIf(floorMin.HasValue, e => e.Floor >= floorMin!.Value)
                    .WhereIf(floorMax.HasValue, e => e.Floor <= floorMax!.Value)
                    .WhereIf(maximumNumberOfGuestMin.HasValue, e => e.MaximumNumberOfGuest >= maximumNumberOfGuestMin!.Value)
                    .WhereIf(maximumNumberOfGuestMax.HasValue, e => e.MaximumNumberOfGuest <= maximumNumberOfGuestMax!.Value)
                    .WhereIf(livingroomsMin.HasValue, e => e.Livingrooms >= livingroomsMin!.Value)
                    .WhereIf(livingroomsMax.HasValue, e => e.Livingrooms <= livingroomsMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(propertyDescription), e => e.PropertyDescription.Contains(propertyDescription))
                    .WhereIf(!string.IsNullOrWhiteSpace(hourseRules), e => e.HourseRules.Contains(hourseRules))
                    .WhereIf(!string.IsNullOrWhiteSpace(importantInformation), e => e.ImportantInformation.Contains(importantInformation))
                    .WhereIf(!string.IsNullOrWhiteSpace(address), e => e.Address.Contains(address))
                    .WhereIf(!string.IsNullOrWhiteSpace(streetAndBuildingNumber), e => e.StreetAndBuildingNumber.Contains(streetAndBuildingNumber))
                    .WhereIf(!string.IsNullOrWhiteSpace(landMark), e => e.LandMark.Contains(landMark))
                    .WhereIf(pricePerNightMin.HasValue, e => e.PricePerNight >= pricePerNightMin!.Value)
                    .WhereIf(pricePerNightMax.HasValue, e => e.PricePerNight <= pricePerNightMax!.Value)
                    .WhereIf(isActive.HasValue, e => e.IsActive == isActive);
        }
    }
}