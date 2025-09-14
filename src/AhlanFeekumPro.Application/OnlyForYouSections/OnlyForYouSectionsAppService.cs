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
using AhlanFeekumPro.OnlyForYouSections;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using AhlanFeekumPro.Shared;
using Volo.Abp.BlobStoring;

namespace AhlanFeekumPro.OnlyForYouSections
{

    [Authorize(AhlanFeekumProPermissions.OnlyForYouSections.Default)]
    public abstract class OnlyForYouSectionsAppServiceBase : AhlanFeekumProAppService
    {
        protected IDistributedCache<OnlyForYouSectionDownloadTokenCacheItem, string> _downloadTokenCache;
        protected IOnlyForYouSectionRepository _onlyForYouSectionRepository;
        protected OnlyForYouSectionManager _onlyForYouSectionManager;
        protected IRepository<AppFileDescriptors.AppFileDescriptor, Guid> _appFileDescriptorRepository;
        protected IBlobContainer<OnlyForYouSectionFileContainer> _blobContainer;

        public OnlyForYouSectionsAppServiceBase(IOnlyForYouSectionRepository onlyForYouSectionRepository, OnlyForYouSectionManager onlyForYouSectionManager, IDistributedCache<OnlyForYouSectionDownloadTokenCacheItem, string> downloadTokenCache, IRepository<AppFileDescriptors.AppFileDescriptor, Guid> appFileDescriptorRepository, IBlobContainer<OnlyForYouSectionFileContainer> blobContainer)
        {
            _downloadTokenCache = downloadTokenCache;
            _onlyForYouSectionRepository = onlyForYouSectionRepository;
            _onlyForYouSectionManager = onlyForYouSectionManager;
            _appFileDescriptorRepository = appFileDescriptorRepository;
            _blobContainer = blobContainer;
        }

        public virtual async Task<PagedResultDto<OnlyForYouSectionDto>> GetListAsync(GetOnlyForYouSectionsInput input)
        {
            var totalCount = await _onlyForYouSectionRepository.GetCountAsync(input.FilterText);
            var items = await _onlyForYouSectionRepository.GetListAsync(input.FilterText, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<OnlyForYouSectionDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<OnlyForYouSection>, List<OnlyForYouSectionDto>>(items)
            };
        }

        public virtual async Task<OnlyForYouSectionDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<OnlyForYouSection, OnlyForYouSectionDto>(await _onlyForYouSectionRepository.GetAsync(id));
        }

        [Authorize(AhlanFeekumProPermissions.OnlyForYouSections.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _onlyForYouSectionRepository.DeleteAsync(id);
        }

        [Authorize(AhlanFeekumProPermissions.OnlyForYouSections.Create)]
        public virtual async Task<OnlyForYouSectionDto> CreateAsync(OnlyForYouSectionCreateDto input)
        {

            var onlyForYouSection = await _onlyForYouSectionManager.CreateAsync(
            input.FirstPhotoId, input.SecondPhotoId, input.ThirdPhotoId
            );

            return ObjectMapper.Map<OnlyForYouSection, OnlyForYouSectionDto>(onlyForYouSection);
        }

        [Authorize(AhlanFeekumProPermissions.OnlyForYouSections.Edit)]
        public virtual async Task<OnlyForYouSectionDto> UpdateAsync(Guid id, OnlyForYouSectionUpdateDto input)
        {

            var onlyForYouSection = await _onlyForYouSectionManager.UpdateAsync(
            id,
            input.FirstPhotoId, input.SecondPhotoId, input.ThirdPhotoId, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<OnlyForYouSection, OnlyForYouSectionDto>(onlyForYouSection);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(OnlyForYouSectionExcelDownloadDto input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _onlyForYouSectionRepository.GetListAsync(input.FilterText);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<OnlyForYouSection>, List<OnlyForYouSectionExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "OnlyForYouSections.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(AhlanFeekumProPermissions.OnlyForYouSections.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> onlyforyousectionIds)
        {
            await _onlyForYouSectionRepository.DeleteManyAsync(onlyforyousectionIds);
        }

        [Authorize(AhlanFeekumProPermissions.OnlyForYouSections.Delete)]
        public virtual async Task DeleteAllAsync(GetOnlyForYouSectionsInput input)
        {
            await _onlyForYouSectionRepository.DeleteAllAsync(input.FilterText);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetFileAsync(GetFileInput input)
        {
            var downloadToken = await _downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var fileDescriptor = await _appFileDescriptorRepository.GetAsync(input.FileId);
            var stream = await _blobContainer.GetAsync(fileDescriptor.Id.ToString("N"));

            return new RemoteStreamContent(stream, fileDescriptor.Name, fileDescriptor.MimeType);
        }

        public virtual async Task<AppFileDescriptorDto> UploadFileAsync(IRemoteStreamContent input)
        {
            var id = GuidGenerator.Create();
            var fileDescriptor = await _appFileDescriptorRepository.InsertAsync(new AppFileDescriptors.AppFileDescriptor(id, input.FileName, input.ContentType));

            await _blobContainer.SaveAsync(fileDescriptor.Id.ToString("N"), input.GetStream());

            return ObjectMapper.Map<AppFileDescriptors.AppFileDescriptor, AppFileDescriptorDto>(fileDescriptor);
        }

        public virtual async Task<AhlanFeekumPro.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _downloadTokenCache.SetAsync(
                token,
                new OnlyForYouSectionDownloadTokenCacheItem { Token = token },
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