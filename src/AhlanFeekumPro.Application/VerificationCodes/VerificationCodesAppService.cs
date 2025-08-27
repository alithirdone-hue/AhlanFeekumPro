using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using AhlanFeekumPro.Permissions;
using AhlanFeekumPro.VerificationCodes;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using AhlanFeekumPro.Shared;

namespace AhlanFeekumPro.VerificationCodes
{

    [Authorize(AhlanFeekumProPermissions.VerificationCodes.Default)]
    public abstract class VerificationCodesAppServiceBase : AhlanFeekumProAppService
    {
        protected IDistributedCache<VerificationCodeDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IVerificationCodeRepository _verificationCodeRepository;
        protected VerificationCodeManager _verificationCodeManager;

        public VerificationCodesAppServiceBase(IVerificationCodeRepository verificationCodeRepository, VerificationCodeManager verificationCodeManager, IDistributedCache<VerificationCodeDownloadTokenCacheItem, string> downloadTokenCache)
        {
            _downloadTokenCache = downloadTokenCache;
            _verificationCodeRepository = verificationCodeRepository;
            _verificationCodeManager = verificationCodeManager;

        }

        public virtual async Task<PagedResultDto<VerificationCodeDto>> GetListAsync(GetVerificationCodesInput input)
        {
            var totalCount = await _verificationCodeRepository.GetCountAsync(input.FilterText, input.PhoneOrEmail, input.SecurityCodeMin, input.SecurityCodeMax, input.IsExpired);
            var items = await _verificationCodeRepository.GetListAsync(input.FilterText, input.PhoneOrEmail, input.SecurityCodeMin, input.SecurityCodeMax, input.IsExpired, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<VerificationCodeDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<VerificationCode>, List<VerificationCodeDto>>(items)
            };
        }

        public virtual async Task<VerificationCodeDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<VerificationCode, VerificationCodeDto>(await _verificationCodeRepository.GetAsync(id));
        }

        [Authorize(AhlanFeekumProPermissions.VerificationCodes.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _verificationCodeRepository.DeleteAsync(id);
        }

        [Authorize(AhlanFeekumProPermissions.VerificationCodes.Create)]
        public virtual async Task<VerificationCodeDto> CreateAsync(VerificationCodeCreateDto input)
        {

            var verificationCode = await _verificationCodeManager.CreateAsync(
            input.PhoneOrEmail, input.SecurityCode, input.IsExpired
            );

            return ObjectMapper.Map<VerificationCode, VerificationCodeDto>(verificationCode);
        }

        [Authorize(AhlanFeekumProPermissions.VerificationCodes.Edit)]
        public virtual async Task<VerificationCodeDto> UpdateAsync(Guid id, VerificationCodeUpdateDto input)
        {

            var verificationCode = await _verificationCodeManager.UpdateAsync(
            id,
            input.PhoneOrEmail, input.SecurityCode, input.IsExpired, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<VerificationCode, VerificationCodeDto>(verificationCode);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(VerificationCodeExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _verificationCodeRepository.GetListAsync(input.FilterText, input.PhoneOrEmail, input.SecurityCodeMin, input.SecurityCodeMax, input.IsExpired);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<VerificationCode>, List<VerificationCodeExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "VerificationCodes.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(AhlanFeekumProPermissions.VerificationCodes.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> verificationcodeIds)
        {
            await _verificationCodeRepository.DeleteManyAsync(verificationcodeIds);
        }

        [Authorize(AhlanFeekumProPermissions.VerificationCodes.Delete)]
        public virtual async Task DeleteAllAsync(GetVerificationCodesInput input)
        {
            await _verificationCodeRepository.DeleteAllAsync(input.FilterText, input.PhoneOrEmail, input.SecurityCodeMin, input.SecurityCodeMax, input.IsExpired);
        }
        public virtual async Task<AhlanFeekumPro.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new VerificationCodeDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new AhlanFeekumPro.Shared.DownloadTokenResultDto
            {
                Token = token
            };
        }
    }
}