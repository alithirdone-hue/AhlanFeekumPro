using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AhlanFeekumPro.UserProfiles
{
    public partial interface IUserProfileRepository : IRepository<UserProfile, Guid>
    {

        Task DeleteAllAsync(
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
            CancellationToken cancellationToken = default);
        Task<UserProfileWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<UserProfileWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
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
            CancellationToken cancellationToken = default
        );

        Task<List<UserProfile>> GetListAsync(
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
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
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
            CancellationToken cancellationToken = default);
    }
}