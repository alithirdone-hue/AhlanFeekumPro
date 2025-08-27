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

namespace AhlanFeekumPro.PropertyFeatures
{
    public abstract class EfCorePropertyFeatureRepositoryBase : EfCoreRepository<AhlanFeekumProDbContext, PropertyFeature, Guid>
    {
        public EfCorePropertyFeatureRepositoryBase(IDbContextProvider<AhlanFeekumProDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        string? title = null,
            string? icon = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default)
        {

            var query = await GetQueryableAsync();

            query = ApplyFilter(query, filterText, title, icon, orderMin, orderMax, isActive);

            var ids = query.Select(x => x.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<PropertyFeature>> GetListAsync(
            string? filterText = null,
            string? title = null,
            string? icon = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, title, icon, orderMin, orderMax, isActive);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? PropertyFeatureConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? title = null,
            string? icon = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, title, icon, orderMin, orderMax, isActive);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<PropertyFeature> ApplyFilter(
            IQueryable<PropertyFeature> query,
            string? filterText = null,
            string? title = null,
            string? icon = null,
            int? orderMin = null,
            int? orderMax = null,
            bool? isActive = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Title!.Contains(filterText!) || e.Icon!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(title), e => e.Title.Contains(title))
                    .WhereIf(!string.IsNullOrWhiteSpace(icon), e => e.Icon.Contains(icon))
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin!.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax!.Value)
                    .WhereIf(isActive.HasValue, e => e.IsActive == isActive);
        }
    }
}