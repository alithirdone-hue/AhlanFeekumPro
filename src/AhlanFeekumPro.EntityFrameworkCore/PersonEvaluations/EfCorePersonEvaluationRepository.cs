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

namespace AhlanFeekumPro.PersonEvaluations
{
    public abstract class EfCorePersonEvaluationRepositoryBase : EfCoreRepository<AhlanFeekumProDbContext, PersonEvaluation, Guid>
    {
        public EfCorePersonEvaluationRepositoryBase(IDbContextProvider<AhlanFeekumProDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        int? rateMin = null,
            int? rateMax = null,
            string? comment = null,
            Guid? evaluatorId = null,
            Guid? evaluatedPersonId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, rateMin, rateMax, comment, evaluatorId, evaluatedPersonId);

            var ids = query.Select(x => x.PersonEvaluation.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<PersonEvaluationWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(personEvaluation => new PersonEvaluationWithNavigationProperties
                {
                    PersonEvaluation = personEvaluation,
                    Evaluator = dbContext.Set<UserProfile>().FirstOrDefault(c => c.Id == personEvaluation.EvaluatorId),
                    EvaluatedPerson = dbContext.Set<UserProfile>().FirstOrDefault(c => c.Id == personEvaluation.EvaluatedPersonId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<PersonEvaluationWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            int? rateMin = null,
            int? rateMax = null,
            string? comment = null,
            Guid? evaluatorId = null,
            Guid? evaluatedPersonId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, rateMin, rateMax, comment, evaluatorId, evaluatedPersonId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? PersonEvaluationConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<PersonEvaluationWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from personEvaluation in (await GetDbSetAsync())
                   join evaluator in (await GetDbContextAsync()).Set<UserProfile>() on personEvaluation.EvaluatorId equals evaluator.Id into userProfiles
                   from evaluator in userProfiles.DefaultIfEmpty()
                   join evaluatedPerson in (await GetDbContextAsync()).Set<UserProfile>() on personEvaluation.EvaluatedPersonId equals evaluatedPerson.Id into userProfiles1
                   from evaluatedPerson in userProfiles1.DefaultIfEmpty()
                   select new PersonEvaluationWithNavigationProperties
                   {
                       PersonEvaluation = personEvaluation,
                       Evaluator = evaluator,
                       EvaluatedPerson = evaluatedPerson
                   };
        }

        protected virtual IQueryable<PersonEvaluationWithNavigationProperties> ApplyFilter(
            IQueryable<PersonEvaluationWithNavigationProperties> query,
            string? filterText,
            int? rateMin = null,
            int? rateMax = null,
            string? comment = null,
            Guid? evaluatorId = null,
            Guid? evaluatedPersonId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.PersonEvaluation.Comment!.Contains(filterText!))
                    .WhereIf(rateMin.HasValue, e => e.PersonEvaluation.Rate >= rateMin!.Value)
                    .WhereIf(rateMax.HasValue, e => e.PersonEvaluation.Rate <= rateMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(comment), e => e.PersonEvaluation.Comment.Contains(comment))
                    .WhereIf(evaluatorId != null && evaluatorId != Guid.Empty, e => e.Evaluator != null && e.Evaluator.Id == evaluatorId)
                    .WhereIf(evaluatedPersonId != null && evaluatedPersonId != Guid.Empty, e => e.EvaluatedPerson != null && e.EvaluatedPerson.Id == evaluatedPersonId);
        }

        public virtual async Task<List<PersonEvaluation>> GetListAsync(
            string? filterText = null,
            int? rateMin = null,
            int? rateMax = null,
            string? comment = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, rateMin, rateMax, comment);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? PersonEvaluationConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            int? rateMin = null,
            int? rateMax = null,
            string? comment = null,
            Guid? evaluatorId = null,
            Guid? evaluatedPersonId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, rateMin, rateMax, comment, evaluatorId, evaluatedPersonId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<PersonEvaluation> ApplyFilter(
            IQueryable<PersonEvaluation> query,
            string? filterText = null,
            int? rateMin = null,
            int? rateMax = null,
            string? comment = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Comment!.Contains(filterText!))
                    .WhereIf(rateMin.HasValue, e => e.Rate >= rateMin!.Value)
                    .WhereIf(rateMax.HasValue, e => e.Rate <= rateMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(comment), e => e.Comment.Contains(comment));
        }
    }
}