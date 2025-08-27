using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AhlanFeekumPro.PersonEvaluations
{
    public partial interface IPersonEvaluationRepository : IRepository<PersonEvaluation, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            int? rateMin = null,
            int? rateMax = null,
            string? comment = null,
            Guid? evaluatorId = null,
            Guid? evaluatedPersonId = null,
            CancellationToken cancellationToken = default);
        Task<PersonEvaluationWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<PersonEvaluationWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            int? rateMin = null,
            int? rateMax = null,
            string? comment = null,
            Guid? evaluatorId = null,
            Guid? evaluatedPersonId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<PersonEvaluation>> GetListAsync(
                    string? filterText = null,
                    int? rateMin = null,
                    int? rateMax = null,
                    string? comment = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            int? rateMin = null,
            int? rateMax = null,
            string? comment = null,
            Guid? evaluatorId = null,
            Guid? evaluatedPersonId = null,
            CancellationToken cancellationToken = default);
    }
}