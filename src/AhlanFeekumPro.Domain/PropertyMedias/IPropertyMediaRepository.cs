using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AhlanFeekumPro.PropertyMedias
{
    public partial interface IPropertyMediaRepository : IRepository<PropertyMedia, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? image = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? sitePropertyId = null,
            CancellationToken cancellationToken = default);
        Task<PropertyMediaWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<PropertyMediaWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? image = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? sitePropertyId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<PropertyMedia>> GetListAsync(
                    string? filterText = null,
                    string? image = null,
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
            string? image = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? sitePropertyId = null,
            CancellationToken cancellationToken = default);
    }
}