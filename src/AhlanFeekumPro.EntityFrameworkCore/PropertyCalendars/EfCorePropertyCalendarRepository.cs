using AhlanFeekumPro.SiteProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using AhlanFeekumPro.EntityFrameworkCore;

namespace AhlanFeekumPro.PropertyCalendars
{
    public abstract class EfCorePropertyCalendarRepositoryBase : EfCoreRepository<AhlanFeekumProDbContext, PropertyCalendar, Guid>
    {
        public EfCorePropertyCalendarRepositoryBase(IDbContextProvider<AhlanFeekumProDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        DateOnly? dateMin = null,
            DateOnly? dateMax = null,
            bool? isAvailable = null,
            float? priceMin = null,
            float? priceMax = null,
            string? note = null,
            Guid? sitePropertyId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, dateMin, dateMax, isAvailable, priceMin, priceMax, note, sitePropertyId);

            var ids = query.Select(x => x.PropertyCalendar.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<PropertyCalendarWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(propertyCalendar => new PropertyCalendarWithNavigationProperties
                {
                    PropertyCalendar = propertyCalendar,
                    SiteProperty = dbContext.Set<SiteProperty>().FirstOrDefault(c => c.Id == propertyCalendar.SitePropertyId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<PropertyCalendarWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, dateMin, dateMax, isAvailable, priceMin, priceMax, note, sitePropertyId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? PropertyCalendarConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<PropertyCalendarWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from propertyCalendar in (await GetDbSetAsync())
                   join siteProperty in (await GetDbContextAsync()).Set<SiteProperty>() on propertyCalendar.SitePropertyId equals siteProperty.Id into siteProperties
                   from siteProperty in siteProperties.DefaultIfEmpty()
                   select new PropertyCalendarWithNavigationProperties
                   {
                       PropertyCalendar = propertyCalendar,
                       SiteProperty = siteProperty
                   };
        }

        protected virtual IQueryable<PropertyCalendarWithNavigationProperties> ApplyFilter(
            IQueryable<PropertyCalendarWithNavigationProperties> query,
            string? filterText,
            DateOnly? dateMin = null,
            DateOnly? dateMax = null,
            bool? isAvailable = null,
            float? priceMin = null,
            float? priceMax = null,
            string? note = null,
            Guid? sitePropertyId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.PropertyCalendar.Note!.Contains(filterText!))
                    .WhereIf(dateMin.HasValue, e => e.PropertyCalendar.Date >= dateMin!.Value)
                    .WhereIf(dateMax.HasValue, e => e.PropertyCalendar.Date <= dateMax!.Value)
                    .WhereIf(isAvailable.HasValue, e => e.PropertyCalendar.IsAvailable == isAvailable)
                    .WhereIf(priceMin.HasValue, e => e.PropertyCalendar.Price >= priceMin!.Value)
                    .WhereIf(priceMax.HasValue, e => e.PropertyCalendar.Price <= priceMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(note), e => e.PropertyCalendar.Note.Contains(note))
                    .WhereIf(sitePropertyId != null && sitePropertyId != Guid.Empty, e => e.SiteProperty != null && e.SiteProperty.Id == sitePropertyId);
        }

        public virtual async Task<List<PropertyCalendar>> GetListAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, dateMin, dateMax, isAvailable, priceMin, priceMax, note);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? PropertyCalendarConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            DateOnly? dateMin = null,
            DateOnly? dateMax = null,
            bool? isAvailable = null,
            float? priceMin = null,
            float? priceMax = null,
            string? note = null,
            Guid? sitePropertyId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, dateMin, dateMax, isAvailable, priceMin, priceMax, note, sitePropertyId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<PropertyCalendar> ApplyFilter(
            IQueryable<PropertyCalendar> query,
            string? filterText = null,
            DateOnly? dateMin = null,
            DateOnly? dateMax = null,
            bool? isAvailable = null,
            float? priceMin = null,
            float? priceMax = null,
            string? note = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Note!.Contains(filterText!))
                    .WhereIf(dateMin.HasValue, e => e.Date >= dateMin!.Value)
                    .WhereIf(dateMax.HasValue, e => e.Date <= dateMax!.Value)
                    .WhereIf(isAvailable.HasValue, e => e.IsAvailable == isAvailable)
                    .WhereIf(priceMin.HasValue, e => e.Price >= priceMin!.Value)
                    .WhereIf(priceMax.HasValue, e => e.Price <= priceMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(note), e => e.Note.Contains(note));
        }
    }
}