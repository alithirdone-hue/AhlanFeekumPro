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

namespace AhlanFeekumPro.PropertyEvaluations
{
    public abstract class EfCorePropertyEvaluationRepositoryBase : EfCoreRepository<AhlanFeekumProDbContext, PropertyEvaluation, Guid>
    {
        public EfCorePropertyEvaluationRepositoryBase(IDbContextProvider<AhlanFeekumProDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, cleanlinessMin, cleanlinessMax, priceAndValueMin, priceAndValueMax, locationMin, locationMax, accuracyMin, accuracyMax, attitudeMin, attitudeMax, ratingComment, userProfileId, sitePropertyId);

            var ids = query.Select(x => x.PropertyEvaluation.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<PropertyEvaluationWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(propertyEvaluation => new PropertyEvaluationWithNavigationProperties
                {
                    PropertyEvaluation = propertyEvaluation,
                    UserProfile = dbContext.Set<UserProfile>().FirstOrDefault(c => c.Id == propertyEvaluation.UserProfileId),
                    SiteProperty = dbContext.Set<SiteProperty>().FirstOrDefault(c => c.Id == propertyEvaluation.SitePropertyId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<PropertyEvaluationWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, cleanlinessMin, cleanlinessMax, priceAndValueMin, priceAndValueMax, locationMin, locationMax, accuracyMin, accuracyMax, attitudeMin, attitudeMax, ratingComment, userProfileId, sitePropertyId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? PropertyEvaluationConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<PropertyEvaluationWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from propertyEvaluation in (await GetDbSetAsync())
                   join userProfile in (await GetDbContextAsync()).Set<UserProfile>() on propertyEvaluation.UserProfileId equals userProfile.Id into userProfiles
                   from userProfile in userProfiles.DefaultIfEmpty()
                   join siteProperty in (await GetDbContextAsync()).Set<SiteProperty>() on propertyEvaluation.SitePropertyId equals siteProperty.Id into siteProperties
                   from siteProperty in siteProperties.DefaultIfEmpty()
                   select new PropertyEvaluationWithNavigationProperties
                   {
                       PropertyEvaluation = propertyEvaluation,
                       UserProfile = userProfile,
                       SiteProperty = siteProperty
                   };
        }

        protected virtual IQueryable<PropertyEvaluationWithNavigationProperties> ApplyFilter(
            IQueryable<PropertyEvaluationWithNavigationProperties> query,
            string? filterText,
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
            Guid? sitePropertyId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.PropertyEvaluation.RatingComment!.Contains(filterText!))
                    .WhereIf(cleanlinessMin.HasValue, e => e.PropertyEvaluation.Cleanliness >= cleanlinessMin!.Value)
                    .WhereIf(cleanlinessMax.HasValue, e => e.PropertyEvaluation.Cleanliness <= cleanlinessMax!.Value)
                    .WhereIf(priceAndValueMin.HasValue, e => e.PropertyEvaluation.PriceAndValue >= priceAndValueMin!.Value)
                    .WhereIf(priceAndValueMax.HasValue, e => e.PropertyEvaluation.PriceAndValue <= priceAndValueMax!.Value)
                    .WhereIf(locationMin.HasValue, e => e.PropertyEvaluation.Location >= locationMin!.Value)
                    .WhereIf(locationMax.HasValue, e => e.PropertyEvaluation.Location <= locationMax!.Value)
                    .WhereIf(accuracyMin.HasValue, e => e.PropertyEvaluation.Accuracy >= accuracyMin!.Value)
                    .WhereIf(accuracyMax.HasValue, e => e.PropertyEvaluation.Accuracy <= accuracyMax!.Value)
                    .WhereIf(attitudeMin.HasValue, e => e.PropertyEvaluation.Attitude >= attitudeMin!.Value)
                    .WhereIf(attitudeMax.HasValue, e => e.PropertyEvaluation.Attitude <= attitudeMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(ratingComment), e => e.PropertyEvaluation.RatingComment.Contains(ratingComment))
                    .WhereIf(userProfileId != null && userProfileId != Guid.Empty, e => e.UserProfile != null && e.UserProfile.Id == userProfileId)
                    .WhereIf(sitePropertyId != null && sitePropertyId != Guid.Empty, e => e.SiteProperty != null && e.SiteProperty.Id == sitePropertyId);
        }

        public virtual async Task<List<PropertyEvaluation>> GetListAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, cleanlinessMin, cleanlinessMax, priceAndValueMin, priceAndValueMax, locationMin, locationMax, accuracyMin, accuracyMax, attitudeMin, attitudeMax, ratingComment);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? PropertyEvaluationConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, cleanlinessMin, cleanlinessMax, priceAndValueMin, priceAndValueMax, locationMin, locationMax, accuracyMin, accuracyMax, attitudeMin, attitudeMax, ratingComment, userProfileId, sitePropertyId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<PropertyEvaluation> ApplyFilter(
            IQueryable<PropertyEvaluation> query,
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
            string? ratingComment = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.RatingComment!.Contains(filterText!))
                    .WhereIf(cleanlinessMin.HasValue, e => e.Cleanliness >= cleanlinessMin!.Value)
                    .WhereIf(cleanlinessMax.HasValue, e => e.Cleanliness <= cleanlinessMax!.Value)
                    .WhereIf(priceAndValueMin.HasValue, e => e.PriceAndValue >= priceAndValueMin!.Value)
                    .WhereIf(priceAndValueMax.HasValue, e => e.PriceAndValue <= priceAndValueMax!.Value)
                    .WhereIf(locationMin.HasValue, e => e.Location >= locationMin!.Value)
                    .WhereIf(locationMax.HasValue, e => e.Location <= locationMax!.Value)
                    .WhereIf(accuracyMin.HasValue, e => e.Accuracy >= accuracyMin!.Value)
                    .WhereIf(accuracyMax.HasValue, e => e.Accuracy <= accuracyMax!.Value)
                    .WhereIf(attitudeMin.HasValue, e => e.Attitude >= attitudeMin!.Value)
                    .WhereIf(attitudeMax.HasValue, e => e.Attitude <= attitudeMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(ratingComment), e => e.RatingComment.Contains(ratingComment));
        }
    }
}