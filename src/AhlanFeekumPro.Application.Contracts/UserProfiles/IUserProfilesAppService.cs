using AhlanFeekumPro.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using AhlanFeekumPro.Shared;

namespace AhlanFeekumPro.UserProfiles
{
    public partial interface IUserProfilesAppService : IApplicationService
    {

        Task<PagedResultDto<UserProfileWithNavigationPropertiesDto>> GetListAsync(GetUserProfilesInput input);

        Task<UserProfileWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<UserProfileDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetIdentityRoleLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetIdentityUserLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<UserProfileDto> CreateAsync(UserProfileCreateDto input);

        Task<UserProfileDto> UpdateAsync(Guid id, UserProfileUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(UserProfileExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> userprofileIds);

        Task DeleteAllAsync(GetUserProfilesInput input);
        Task<AhlanFeekumPro.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}