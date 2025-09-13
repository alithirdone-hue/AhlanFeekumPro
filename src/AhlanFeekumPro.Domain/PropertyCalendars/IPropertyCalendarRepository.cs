using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AhlanFeekumPro.PropertyCalendars
{
    public partial interface IPropertyCalendarRepository : IRepository<PropertyCalendar, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            DateOnly? dateMin = null,
            DateOnly? dateMax = null,
            bool? isAvailable = null,
            float? priceMin = null,
            float? priceMax = null,
            string? note = null,
            Guid? sitePropertyId = null,
            CancellationToken cancellationToken = default);
        Task<PropertyCalendarWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<PropertyCalendarWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            DateOnly? dateMin = null,
            DateOnly? dateMax = null,
            bool? isAvailable = null,
            float? priceMin = null,
            float? priceMax = null,
            string? note = null,
            Guid? sitePropertyId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<PropertyCalendar>> GetListAsync(
                    string? filterText = null,
                    DateOnly? dateMin = null,
                    DateOnly? dateMax = null,
                    bool? isAvailable = null,
                    float? priceMin = null,
                    float? priceMax = null,
                    string? note = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            DateOnly? dateMin = null,
            DateOnly? dateMax = null,
            bool? isAvailable = null,
            float? priceMin = null,
            float? priceMax = null,
            string? note = null,
            Guid? sitePropertyId = null,
            CancellationToken cancellationToken = default);
    }
}