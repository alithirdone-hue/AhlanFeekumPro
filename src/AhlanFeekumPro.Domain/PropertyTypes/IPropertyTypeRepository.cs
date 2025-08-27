using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AhlanFeekumPro.PropertyTypes
{
    public partial interface IPropertyTypeRepository : IRepository<PropertyType, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? title = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default);
        Task<List<PropertyType>> GetListAsync(
                    string? filterText = null,
                    string? title = null,
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
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default);
    }
}