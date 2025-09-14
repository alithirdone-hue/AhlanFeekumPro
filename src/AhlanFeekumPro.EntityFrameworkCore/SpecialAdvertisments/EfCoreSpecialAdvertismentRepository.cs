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

namespace AhlanFeekumPro.SpecialAdvertisments
{
    public abstract class EfCoreSpecialAdvertismentRepositoryBase : EfCoreRepository<AhlanFeekumProDbContext, SpecialAdvertisment, Guid>
    {
        public EfCoreSpecialAdvertismentRepositoryBase(IDbContextProvider<AhlanFeekumProDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? sitePropertyId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, orderMin, orderMax, isActive, sitePropertyId);

            var ids = query.Select(x => x.SpecialAdvertisment.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<SpecialAdvertismentWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(specialAdvertisment => new SpecialAdvertismentWithNavigationProperties
                {
                    SpecialAdvertisment = specialAdvertisment,
                    SiteProperty = dbContext.Set<SiteProperty>().FirstOrDefault(c => c.Id == specialAdvertisment.SitePropertyId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<SpecialAdvertismentWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
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
            query = ApplyFilter(query, filterText, orderMin, orderMax, isActive, sitePropertyId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? SpecialAdvertismentConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<SpecialAdvertismentWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from specialAdvertisment in (await GetDbSetAsync())
                   join siteProperty in (await GetDbContextAsync()).Set<SiteProperty>() on specialAdvertisment.SitePropertyId equals siteProperty.Id into siteProperties
                   from siteProperty in siteProperties.DefaultIfEmpty()
                   select new SpecialAdvertismentWithNavigationProperties
                   {
                       SpecialAdvertisment = specialAdvertisment,
                       SiteProperty = siteProperty
                   };
        }

        protected virtual IQueryable<SpecialAdvertismentWithNavigationProperties> ApplyFilter(
            IQueryable<SpecialAdvertismentWithNavigationProperties> query,
            string? filterText,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? sitePropertyId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => true)
                    .WhereIf(orderMin.HasValue, e => e.SpecialAdvertisment.Order >= orderMin!.Value)
                    .WhereIf(orderMax.HasValue, e => e.SpecialAdvertisment.Order <= orderMax!.Value)
                    .WhereIf(isActive.HasValue, e => e.SpecialAdvertisment.IsActive == isActive)
                    .WhereIf(sitePropertyId != null && sitePropertyId != Guid.Empty, e => e.SiteProperty != null && e.SiteProperty.Id == sitePropertyId);
        }

        public virtual async Task<List<SpecialAdvertisment>> GetListAsync(
            string? filterText = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, orderMin, orderMax, isActive);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? SpecialAdvertismentConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            Guid? sitePropertyId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, orderMin, orderMax, isActive, sitePropertyId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<SpecialAdvertisment> ApplyFilter(
            IQueryable<SpecialAdvertisment> query,
            string? filterText = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => true)
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin!.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax!.Value)
                    .WhereIf(isActive.HasValue, e => e.IsActive == isActive);
        }
    }
}