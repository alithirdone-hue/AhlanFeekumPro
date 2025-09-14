using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AhlanFeekumPro.SpecialAdvertisments
{
    public partial interface ISpecialAdvertismentRepository : IRepository<SpecialAdvertisment, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? sitePropertyId = null,
            CancellationToken cancellationToken = default);
        Task<SpecialAdvertismentWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<SpecialAdvertismentWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? sitePropertyId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<SpecialAdvertisment>> GetListAsync(
                    string? filterText = null,
                    int? orderMin = null,
                    int? orderMax = null,
                    bool? isActive = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? sitePropertyId = null,
            CancellationToken cancellationToken = default);
    }
}