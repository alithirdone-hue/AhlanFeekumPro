using AhlanFeekumPro.SiteProperties;
using AhlanFeekumPro.UserProfiles;
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

namespace AhlanFeekumPro.FavoriteProperties
{
    public abstract class EfCoreFavoritePropertyRepositoryBase : EfCoreRepository<AhlanFeekumProDbContext, FavoriteProperty, Guid>
    {
        public EfCoreFavoritePropertyRepositoryBase(IDbContextProvider<AhlanFeekumProDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        Guid? userProfileId = null,
            Guid? sitePropertyId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, userProfileId, sitePropertyId);

            var ids = query.Select(x => x.FavoriteProperty.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<FavoritePropertyWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(favoriteProperty => new FavoritePropertyWithNavigationProperties
                {
                    FavoriteProperty = favoriteProperty,
                    UserProfile = dbContext.Set<UserProfile>().FirstOrDefault(c => c.Id == favoriteProperty.UserProfileId),
                    SiteProperty = dbContext.Set<SiteProperty>().FirstOrDefault(c => c.Id == favoriteProperty.SitePropertyId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<FavoritePropertyWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            Guid? userProfileId = null,
            Guid? sitePropertyId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, userProfileId, sitePropertyId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? FavoritePropertyConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<FavoritePropertyWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from favoriteProperty in (await GetDbSetAsync())
                   join userProfile in (await GetDbContextAsync()).Set<UserProfile>() on favoriteProperty.UserProfileId equals userProfile.Id into userProfiles
                   from userProfile in userProfiles.DefaultIfEmpty()
                   join siteProperty in (await GetDbContextAsync()).Set<SiteProperty>() on favoriteProperty.SitePropertyId equals siteProperty.Id into siteProperties
                   from siteProperty in siteProperties.DefaultIfEmpty()
                   select new FavoritePropertyWithNavigationProperties
                   {
                       FavoriteProperty = favoriteProperty,
                       UserProfile = userProfile,
                       SiteProperty = siteProperty
                   };
        }

        protected virtual IQueryable<FavoritePropertyWithNavigationProperties> ApplyFilter(
            IQueryable<FavoritePropertyWithNavigationProperties> query,
            string? filterText,
            Guid? userProfileId = null,
            Guid? sitePropertyId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => true)
                    .WhereIf(userProfileId != null && userProfileId != Guid.Empty, e => e.UserProfile != null && e.UserProfile.Id == userProfileId)
                    .WhereIf(sitePropertyId != null && sitePropertyId != Guid.Empty, e => e.SiteProperty != null && e.SiteProperty.Id == sitePropertyId);
        }

        public virtual async Task<List<FavoriteProperty>> GetListAsync(
            string? filterText = null,

            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? FavoritePropertyConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            Guid? userProfileId = null,
            Guid? sitePropertyId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, userProfileId, sitePropertyId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<FavoriteProperty> ApplyFilter(
            IQueryable<FavoriteProperty> query,
            string? filterText = null
)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => true)
;
        }
    }
}