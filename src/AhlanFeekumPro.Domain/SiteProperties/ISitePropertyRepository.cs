using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AhlanFeekumPro.SiteProperties
{
    public partial interface ISitePropertyRepository : IRepository<SiteProperty, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? propertyTitle = null,
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
            Guid? propertyFeatureId = null,
            CancellationToken cancellationToken = default);
        Task<SitePropertyWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<SitePropertyWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? propertyTitle = null,
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
            Guid? propertyFeatureId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<SiteProperty>> GetListAsync(
                    string? filterText = null,
                    string? propertyTitle = null,
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
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? propertyTitle = null,
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
            Guid? propertyFeatureId = null,
            CancellationToken cancellationToken = default);
    }
}