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

namespace AhlanFeekumPro.OnlyForYouSections
{
    public abstract class EfCoreOnlyForYouSectionRepositoryBase : EfCoreRepository<AhlanFeekumProDbContext, OnlyForYouSection, Guid>
    {
        public EfCoreOnlyForYouSectionRepositoryBase(IDbContextProvider<AhlanFeekumProDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        string? firstPhoto = null,
            string? secondPhoto = null,
            string? thirdPhoto = null,
            CancellationToken cancellationToken = default)
        {

            var query = await GetQueryableAsync();

            query = ApplyFilter(query, filterText, firstPhoto, secondPhoto, thirdPhoto);

            var ids = query.Select(x => x.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<OnlyForYouSection>> GetListAsync(
            string? filterText = null,
            string? firstPhoto = null,
            string? secondPhoto = null,
            string? thirdPhoto = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, firstPhoto, secondPhoto, thirdPhoto);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? OnlyForYouSectionConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? firstPhoto = null,
            string? secondPhoto = null,
            string? thirdPhoto = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetDbSetAsync()), filterText, firstPhoto, secondPhoto, thirdPhoto);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<OnlyForYouSection> ApplyFilter(
            IQueryable<OnlyForYouSection> query,
            string? filterText = null,
            string? firstPhoto = null,
            string? secondPhoto = null,
            string? thirdPhoto = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.FirstPhoto!.Contains(filterText!) || e.SecondPhoto!.Contains(filterText!) || e.ThirdPhoto!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(firstPhoto), e => e.FirstPhoto.Contains(firstPhoto))
                    .WhereIf(!string.IsNullOrWhiteSpace(secondPhoto), e => e.SecondPhoto.Contains(secondPhoto))
                    .WhereIf(!string.IsNullOrWhiteSpace(thirdPhoto), e => e.ThirdPhoto.Contains(thirdPhoto));
        }
    }
}