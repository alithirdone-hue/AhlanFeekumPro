using AhlanFeekumPro.PropertyFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace AhlanFeekumPro.SiteProperties
{
    public abstract class SitePropertyManagerBase : DomainService
    {
        protected ISitePropertyRepository _sitePropertyRepository;
        protected IRepository<PropertyFeature, Guid> _propertyFeatureRepository;

        public SitePropertyManagerBase(ISitePropertyRepository sitePropertyRepository,
        IRepository<PropertyFeature, Guid> propertyFeatureRepository)
        {
            _sitePropertyRepository = sitePropertyRepository;
            _propertyFeatureRepository = propertyFeatureRepository;
        }

        public virtual async Task<SiteProperty> CreateAsync(
        List<Guid> propertyFeatureIds,
        Guid propertyTypeId, string propertyTitle, int bedrooms, int bathrooms, int numberOfBed, int floor, int maximumNumberOfGuest, int livingrooms, string propertyDescription, int pricePerNight, bool isActive, string? hourseRules = null, string? importantInformation = null, string? address = null, string? streetAndBuildingNumber = null, string? landMark = null)
        {
            Check.NotNull(propertyTypeId, nameof(propertyTypeId));
            Check.NotNullOrWhiteSpace(propertyTitle, nameof(propertyTitle));
            Check.NotNullOrWhiteSpace(propertyDescription, nameof(propertyDescription));

            var siteProperty = new SiteProperty(
             GuidGenerator.Create(),
             propertyTypeId, propertyTitle, bedrooms, bathrooms, numberOfBed, floor, maximumNumberOfGuest, livingrooms, propertyDescription, pricePerNight, isActive, hourseRules, importantInformation, address, streetAndBuildingNumber, landMark
             );

            await SetPropertyFeaturesAsync(siteProperty, propertyFeatureIds);

            return await _sitePropertyRepository.InsertAsync(siteProperty);
        }

        public virtual async Task<SiteProperty> UpdateAsync(
            Guid id,
            List<Guid> propertyFeatureIds,
        Guid propertyTypeId, string propertyTitle, int bedrooms, int bathrooms, int numberOfBed, int floor, int maximumNumberOfGuest, int livingrooms, string propertyDescription, int pricePerNight, bool isActive, string? hourseRules = null, string? importantInformation = null, string? address = null, string? streetAndBuildingNumber = null, string? landMark = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(propertyTypeId, nameof(propertyTypeId));
            Check.NotNullOrWhiteSpace(propertyTitle, nameof(propertyTitle));
            Check.NotNullOrWhiteSpace(propertyDescription, nameof(propertyDescription));

            var queryable = await _sitePropertyRepository.WithDetailsAsync(x => x.PropertyFeatures);
            var query = queryable.Where(x => x.Id == id);

            var siteProperty = await AsyncExecuter.FirstOrDefaultAsync(query);

            siteProperty.PropertyTypeId = propertyTypeId;
            siteProperty.PropertyTitle = propertyTitle;
            siteProperty.Bedrooms = bedrooms;
            siteProperty.Bathrooms = bathrooms;
            siteProperty.NumberOfBed = numberOfBed;
            siteProperty.Floor = floor;
            siteProperty.MaximumNumberOfGuest = maximumNumberOfGuest;
            siteProperty.Livingrooms = livingrooms;
            siteProperty.PropertyDescription = propertyDescription;
            siteProperty.PricePerNight = pricePerNight;
            siteProperty.IsActive = isActive;
            siteProperty.HourseRules = hourseRules;
            siteProperty.ImportantInformation = importantInformation;
            siteProperty.Address = address;
            siteProperty.StreetAndBuildingNumber = streetAndBuildingNumber;
            siteProperty.LandMark = landMark;

            await SetPropertyFeaturesAsync(siteProperty, propertyFeatureIds);

            siteProperty.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _sitePropertyRepository.UpdateAsync(siteProperty);
        }

        private async Task SetPropertyFeaturesAsync(SiteProperty siteProperty, List<Guid> propertyFeatureIds)
        {
            if (propertyFeatureIds == null || !propertyFeatureIds.Any())
            {
                siteProperty.RemoveAllPropertyFeatures();
                return;
            }

            var query = (await _propertyFeatureRepository.GetQueryableAsync())
                .Where(x => propertyFeatureIds.Contains(x.Id))
                .Select(x => x.Id);

            var propertyFeatureIdsInDb = await AsyncExecuter.ToListAsync(query);
            if (!propertyFeatureIdsInDb.Any())
            {
                return;
            }

            siteProperty.RemoveAllPropertyFeaturesExceptGivenIds(propertyFeatureIdsInDb);

            foreach (var propertyFeatureId in propertyFeatureIdsInDb)
            {
                siteProperty.AddPropertyFeature(propertyFeatureId);
            }
        }

    }
}