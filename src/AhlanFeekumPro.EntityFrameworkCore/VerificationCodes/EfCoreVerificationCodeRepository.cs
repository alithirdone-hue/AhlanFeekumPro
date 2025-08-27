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

namespace AhlanFeekumPro.VerificationCodes
{
    public abstract class EfCoreVerificationCodeRepositoryBase : EfCoreRepository<AhlanFeekumProDbContext, VerificationCode, Guid>
    {
        public EfCoreVerificationCodeRepositoryBase(IDbContextProvider<AhlanFeekumProDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        string? phoneOrEmail = null,
            int? securityCodeMin = null,
            int? securityCodeMax = null,
            bool? isExpired = null,
            CancellationToken cancellationToken = default)
        {

            var query = await GetQueryableAsync();

            query = ApplyFilter(query, filterText, phoneOrEmail, securityCodeMin, securityCodeMax, isExpired);

            var ids = query.Select(x => x.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<VerificationCode>> GetListAsync(
            string? filterText = null,
            string? phoneOrEmail = null,
            int? securityCodeMin = null,
            int? securityCodeMax = null,
            bool? isExpired = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, phoneOrEmail, securityCodeMin, securityCodeMax, isExpired);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? VerificationCodeConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? phoneOrEmail = null,
            int? securityCodeMin = null,
            int? securityCodeMax = null,
            bool? isExpired = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, phoneOrEmail, securityCodeMin, securityCodeMax, isExpired);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<VerificationCode> ApplyFilter(
            IQueryable<VerificationCode> query,
            string? filterText = null,
            string? phoneOrEmail = null,
            int? securityCodeMin = null,
            int? securityCodeMax = null,
            bool? isExpired = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.PhoneOrEmail!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(phoneOrEmail), e => e.PhoneOrEmail.Contains(phoneOrEmail))
                    .WhereIf(securityCodeMin.HasValue, e => e.SecurityCode >= securityCodeMin!.Value)
                    .WhereIf(securityCodeMax.HasValue, e => e.SecurityCode <= securityCodeMax!.Value)
                    .WhereIf(isExpired.HasValue, e => e.IsExpired == isExpired);
        }
    }
}