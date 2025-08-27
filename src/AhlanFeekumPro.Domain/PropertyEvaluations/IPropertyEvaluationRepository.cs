using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AhlanFeekumPro.PropertyEvaluations
{
    public partial interface IPropertyEvaluationRepository : IRepository<PropertyEvaluation, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            int? cleanlinessMin = null,
            int? cleanlinessMax = null,
            int? priceAndValueMin = null,
            int? priceAndValueMax = null,
            int? locationMin = null,
            int? locationMax = null,
            int? accuracyMin = null,
            int? accuracyMax = null,
            int? attitudeMin = null,
            int? attitudeMax = null,
            string? ratingComment = null,
            Guid? userProfileId = null,
            Guid? sitePropertyId = null,
            CancellationToken cancellationToken = default);
        Task<PropertyEvaluationWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<PropertyEvaluationWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            int? cleanlinessMin = null,
            int? cleanlinessMax = null,
            int? priceAndValueMin = null,
            int? priceAndValueMax = null,
            int? locationMin = null,
            int? locationMax = null,
            int? accuracyMin = null,
            int? accuracyMax = null,
            int? attitudeMin = null,
            int? attitudeMax = null,
            string? ratingComment = null,
            Guid? userProfileId = null,
            Guid? sitePropertyId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<PropertyEvaluation>> GetListAsync(
                    string? filterText = null,
                    int? cleanlinessMin = null,
                    int? cleanlinessMax = null,
                    int? priceAndValueMin = null,
                    int? priceAndValueMax = null,
                    int? locationMin = null,
                    int? locationMax = null,
                    int? accuracyMin = null,
                    int? accuracyMax = null,
                    int? attitudeMin = null,
                    int? attitudeMax = null,
                    string? ratingComment = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            int? cleanlinessMin = null,
            int? cleanlinessMax = null,
            int? priceAndValueMin = null,
            int? priceAndValueMax = null,
            int? locationMin = null,
            int? locationMax = null,
            int? accuracyMin = null,
            int? accuracyMax = null,
            int? attitudeMin = null,
            int? attitudeMax = null,
            string? ratingComment = null,
            Guid? userProfileId = null,
            Guid? sitePropertyId = null,
            CancellationToken cancellationToken = default);
    }
}