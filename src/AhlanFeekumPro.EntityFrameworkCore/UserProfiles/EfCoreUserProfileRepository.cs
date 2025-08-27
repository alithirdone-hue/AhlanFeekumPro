using Volo.Abp.Identity;
using Volo.Abp.Identity;
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

namespace AhlanFeekumPro.UserProfiles
{
    public abstract class EfCoreUserProfileRepositoryBase : EfCoreRepository<AhlanFeekumProDbContext, UserProfile, Guid>
    {
        public EfCoreUserProfileRepositoryBase(IDbContextProvider<AhlanFeekumProDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        string? name = null,
            string? email = null,
            string? phoneNumber = null,
            string? latitude = null,
            string? longitude = null,
            string? address = null,
            string? profilePhoto = null,
            bool? isSuperHost = null,
            Guid? identityRoleId = null,
            Guid? identityUserId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();

            query = ApplyFilter(query, filterText, name, email, phoneNumber, latitude, longitude, address, profilePhoto, isSuperHost, identityRoleId, identityUserId);

            var ids = query.Select(x => x.UserProfile.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<UserProfileWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(userProfile => new UserProfileWithNavigationProperties
                {
                    UserProfile = userProfile,
                    IdentityRole = dbContext.Set<IdentityRole>().FirstOrDefault(c => c.Id == userProfile.IdentityRoleId),
                    IdentityUser = dbContext.Set<IdentityUser>().FirstOrDefault(c => c.Id == userProfile.IdentityUserId)
                }).FirstOrDefault();
        }

        public virtual async Task<List<UserProfileWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? name = null,
            string? email = null,
            string? phoneNumber = null,
            string? latitude = null,
            string? longitude = null,
            string? address = null,
            string? profilePhoto = null,
            bool? isSuperHost = null,
            Guid? identityRoleId = null,
            Guid? identityUserId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, name, email, phoneNumber, latitude, longitude, address, profilePhoto, isSuperHost, identityRoleId, identityUserId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserProfileConsts.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        protected virtual async Task<IQueryable<UserProfileWithNavigationProperties>> GetQueryForNavigationPropertiesAsync()
        {
            return from userProfile in (await GetDbSetAsync())
                   join identityRole in (await GetDbContextAsync()).Set<IdentityRole>() on userProfile.IdentityRoleId equals identityRole.Id into identityRoles
                   from identityRole in identityRoles.DefaultIfEmpty()
                   join identityUser in (await GetDbContextAsync()).Set<IdentityUser>() on userProfile.IdentityUserId equals identityUser.Id into identityUsers
                   from identityUser in identityUsers.DefaultIfEmpty()
                   select new UserProfileWithNavigationProperties
                   {
                       UserProfile = userProfile,
                       IdentityRole = identityRole,
                       IdentityUser = identityUser
                   };
        }

        protected virtual IQueryable<UserProfileWithNavigationProperties> ApplyFilter(
            IQueryable<UserProfileWithNavigationProperties> query,
            string? filterText,
            string? name = null,
            string? email = null,
            string? phoneNumber = null,
            string? latitude = null,
            string? longitude = null,
            string? address = null,
            string? profilePhoto = null,
            bool? isSuperHost = null,
            Guid? identityRoleId = null,
            Guid? identityUserId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.UserProfile.Name!.Contains(filterText!) || e.UserProfile.Email!.Contains(filterText!) || e.UserProfile.PhoneNumber!.Contains(filterText!) || e.UserProfile.Latitude!.Contains(filterText!) || e.UserProfile.Longitude!.Contains(filterText!) || e.UserProfile.Address!.Contains(filterText!) || e.UserProfile.ProfilePhoto!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.UserProfile.Name.Contains(name))
                    .WhereIf(!string.IsNullOrWhiteSpace(email), e => e.UserProfile.Email.Contains(email))
                    .WhereIf(!string.IsNullOrWhiteSpace(phoneNumber), e => e.UserProfile.PhoneNumber.Contains(phoneNumber))
                    .WhereIf(!string.IsNullOrWhiteSpace(latitude), e => e.UserProfile.Latitude.Contains(latitude))
                    .WhereIf(!string.IsNullOrWhiteSpace(longitude), e => e.UserProfile.Longitude.Contains(longitude))
                    .WhereIf(!string.IsNullOrWhiteSpace(address), e => e.UserProfile.Address.Contains(address))
                    .WhereIf(!string.IsNullOrWhiteSpace(profilePhoto), e => e.UserProfile.ProfilePhoto.Contains(profilePhoto))
                    .WhereIf(isSuperHost.HasValue, e => e.UserProfile.IsSuperHost == isSuperHost)
                    .WhereIf(identityRoleId != null && identityRoleId != Guid.Empty, e => e.IdentityRole != null && e.IdentityRole.Id == identityRoleId)
                    .WhereIf(identityUserId != null && identityUserId != Guid.Empty, e => e.IdentityUser != null && e.IdentityUser.Id == identityUserId);
        }

        public virtual async Task<List<UserProfile>> GetListAsync(
            string? filterText = null,
            string? name = null,
            string? email = null,
            string? phoneNumber = null,
            string? latitude = null,
            string? longitude = null,
            string? address = null,
            string? profilePhoto = null,
            bool? isSuperHost = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetQueryableAsync()), filterText, name, email, phoneNumber, latitude, longitude, address, profilePhoto, isSuperHost);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? UserProfileConsts.GetDefaultSorting(false) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            string? email = null,
            string? phoneNumber = null,
            string? latitude = null,
            string? longitude = null,
            string? address = null,
            string? profilePhoto = null,
            bool? isSuperHost = null,
            Guid? identityRoleId = null,
            Guid? identityUserId = null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, name, email, phoneNumber, latitude, longitude, address, profilePhoto, isSuperHost, identityRoleId, identityUserId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<UserProfile> ApplyFilter(
            IQueryable<UserProfile> query,
            string? filterText = null,
            string? name = null,
            string? email = null,
            string? phoneNumber = null,
            string? latitude = null,
            string? longitude = null,
            string? address = null,
            string? profilePhoto = null,
            bool? isSuperHost = null)
        {
            return query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Name!.Contains(filterText!) || e.Email!.Contains(filterText!) || e.PhoneNumber!.Contains(filterText!) || e.Latitude!.Contains(filterText!) || e.Longitude!.Contains(filterText!) || e.Address!.Contains(filterText!) || e.ProfilePhoto!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name))
                    .WhereIf(!string.IsNullOrWhiteSpace(email), e => e.Email.Contains(email))
                    .WhereIf(!string.IsNullOrWhiteSpace(phoneNumber), e => e.PhoneNumber.Contains(phoneNumber))
                    .WhereIf(!string.IsNullOrWhiteSpace(latitude), e => e.Latitude.Contains(latitude))
                    .WhereIf(!string.IsNullOrWhiteSpace(longitude), e => e.Longitude.Contains(longitude))
                    .WhereIf(!string.IsNullOrWhiteSpace(address), e => e.Address.Contains(address))
                    .WhereIf(!string.IsNullOrWhiteSpace(profilePhoto), e => e.ProfilePhoto.Contains(profilePhoto))
                    .WhereIf(isSuperHost.HasValue, e => e.IsSuperHost == isSuperHost);
        }
    }
}