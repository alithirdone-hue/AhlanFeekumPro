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

namespace AhlanFeekumPro.PropertyMedias
{
    public abstract class EfCorePropertyMediaRepositoryBase : EfCoreRepository<AhlanFeekumProDbContext, PropertyMedia, Guid>
    {
        public EfCorePropertyMediaRepositoryBase(IDbContextProvider<AhlanFeekumProDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        string? image = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? sitePropertyId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, image, orderMin, orderMax, isActive, sitePropertyId);

            var ids = query.Select(x => x.PropertyMedia.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<PropertyMediaWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(propertyMedia => new PropertyMediaWithNavigationProperties
                {
                    PropertyMedia = propertyMedia,
                    SiteProperty = dbContext.Set<SiteProperty>().FirstOrDefault(c => c.Id == propertyMedia.SitePropertyId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<PropertyMediaWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? image = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? sitePropertyId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, image, orderMin, orderMax, isActive, sitePropertyId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? PropertyMediaConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<PropertyMediaWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from propertyMedia in (await GetDbSetAsync())
                   join siteProperty in (await GetDbContextAsync()).Set<SiteProperty>() on propertyMedia.SitePropertyId equals siteProperty.Id into siteProperties
                   from siteProperty in siteProperties.DefaultIfEmpty()
                   select new PropertyMediaWithNavigationProperties
                   {
                       PropertyMedia = propertyMedia,
                       SiteProperty = siteProperty
                   };
        }

        protected virtual IQueryable<PropertyMediaWithNavigationProperties> ApplyFilter(
            IQueryable<PropertyMediaWithNavigationProperties> query,
            string? filterText,
            string? image = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? sitePropertyId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.PropertyMedia.Image!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(image), e => e.PropertyMedia.Image.Contains(image))
                    .WhereIf(orderMin.HasValue, e => e.PropertyMedia.Order >= orderMin!.Value)
                    .WhereIf(orderMax.HasValue, e => e.PropertyMedia.Order <= orderMax!.Value)
                    .WhereIf(isActive.HasValue, e => e.PropertyMedia.isActive == isActive)
                    .WhereIf(sitePropertyId != null && sitePropertyId != Guid.Empty, e => e.SiteProperty != null && e.SiteProperty.Id == sitePropertyId);
        }

        public virtual async Task<List<PropertyMedia>> GetListAsync(
            string? filterText = null,
            string? image = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, image, orderMin, orderMax, isActive);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? PropertyMediaConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? image = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? sitePropertyId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, image, orderMin, orderMax, isActive, sitePropertyId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<PropertyMedia> ApplyFilter(
            IQueryable<PropertyMedia> query,
            string? filterText = null,
            string? image = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Image!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(image), e => e.Image.Contains(image))
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin!.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax!.Value)
                    .WhereIf(isActive.HasValue, e => e.isActive == isActive);
        }
    }
}