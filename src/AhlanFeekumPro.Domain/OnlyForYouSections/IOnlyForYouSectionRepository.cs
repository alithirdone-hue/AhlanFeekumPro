using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AhlanFeekumPro.OnlyForYouSections
{
    public partial interface IOnlyForYouSectionRepository : IRepository<OnlyForYouSection, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? firstPhoto = null,
            string? secondPhoto = null,
            string? thirdPhoto = null,
            CancellationToken cancellationToken = default);
        Task<List<OnlyForYouSection>> GetListAsync(
                    string? filterText = null,
                    string? firstPhoto = null,
                    string? secondPhoto = null,
                    string? thirdPhoto = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? firstPhoto = null,
            string? secondPhoto = null,
            string? thirdPhoto = null,
            CancellationToken cancellationToken = default);
    }
}