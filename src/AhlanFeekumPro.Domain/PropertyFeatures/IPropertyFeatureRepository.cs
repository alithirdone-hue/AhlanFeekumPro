using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AhlanFeekumPro.PropertyFeatures
{
    public partial interface IPropertyFeatureRepository : IRepository<PropertyFeature, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? title = null,
            string? icon = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default);
        Task<List<PropertyFeature>> GetListAsync(
                    string? filterText = null,
                    string? title = null,
                    string? icon = null,
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
            string? title = null,
            string? icon = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default);
    }
}