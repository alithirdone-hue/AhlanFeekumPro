using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AhlanFeekumPro.FavoriteProperties
{
    public partial interface IFavoritePropertyRepository : IRepository<FavoriteProperty, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            Guid? userProfileId = null,
            Guid? sitePropertyId = null,
            CancellationToken cancellationToken = default);
        Task<FavoritePropertyWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<FavoritePropertyWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            Guid? userProfileId = null,
            Guid? sitePropertyId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<FavoriteProperty>> GetListAsync(
                    string? filterText = null,

                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            Guid? userProfileId = null,
            Guid? sitePropertyId = null,
            CancellationToken cancellationToken = default);
    }
}