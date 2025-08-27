using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using AhlanFeekumPro.UserProfiles;
using AhlanFeekumPro.MobileResponses;

namespace AhlanFeekumPro.Controllers.UserProfiles
{
    [RemoteService]
    [Area("app")]
    [ControllerName("UserProfile")]
    [Route("api/app/user-profiles")]

    public class UserProfileController : AbpController
    {
        protected IUserProfilesAppService _userProfilesAppService;
        public UserProfileController(IUserProfilesAppService userProfilesAppService)
        {
            _userProfilesAppService = userProfilesAppService;
        }

        [AllowAnonymous]
        [HttpPost("register-user")]
        public virtual Task<MobileResponseDto> RegisterAsync([FromForm] RegisterCreateDto input)
        {
            return _userProfilesAppService.RegisterAsync(input);
        }

        //[HttpPost("update-user")]
        //public virtual Task<MobileResponseDto> UpdateUserDetailsAsync([FromForm] UserUpdateDto input)
        //{
        //    return _userProfilesAppService.UpdateUserDetailsAsync(input);
        //}

        //[AllowAnonymous]
        //[HttpPost("verify-email-exists")]
        //public virtual Task<MobileResponseDto> VerifyEmailExistsAsync([FromBody] VerifyEmailExistsDto input)
        //{
        //    return _userProfilesAppService.VerifyEmailExistsAsync(input);
        //}

        //[AllowAnonymous]
        //[HttpPost("verify-secret-key")]
        //public virtual Task<MobileResponseDto> VerifySecurityCodeAsync(SecurityNumberCreateDto input)
        //{
        //    return null;
        //    //return _userProfilesAppService.VerifySecrityCodeAsync(input);
        //}


        //[AllowAnonymous]
        //[HttpPost("password-reset")]
        //public virtual Task<MobileResponseDto> ResetPasswordWithoutTokenAsync(PasswordResetRequestDto input)
        //{
        //    return _userProfilesAppService.ResetPasswordWithoutTokenAsync(input);
        //}

        //[AllowAnonymous]
        //[HttpPost("password-change")]
        //public virtual Task<MobileResponseDto> ChangePasswordAsync(PasswordChangeRequestDto input)
        //{
        //    return _userProfilesAppService.ChangePasswordAsync(input);
        //}

        //[HttpGet]
        //[Route("user-profile-details/{id}")]
        //public virtual Task<UserProfileWithDetailsDto> GetWithDetailsAsync(Guid id)
        //{
        //    return _userProfilesAppService.GetWithDetailsAsync(id);
        //}

        //[HttpGet("check-follow-status/{id}")]
        //public Task<bool> CheckFollowStatusAsync(Guid id)
        //{
        //    return _userProfilesAppService.CheckFollowStatusAsync(id);
        //}

 

        //[HttpGet("check-saved-item-status/{id}")]
        //public Task<bool> CheckSavedItemStatusAsync(Guid id)
        //{
        //    return _userProfilesAppService.CheckSavedItemStatusAsync(id);
        //}

    }
}