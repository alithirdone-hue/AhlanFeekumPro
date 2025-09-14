using AhlanFeekumPro.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using AhlanFeekumPro.Shared;

namespace AhlanFeekumPro.SpecialAdvertisments
{
    public partial interface ISpecialAdvertismentsAppService : IApplicationService
    {
        Task<IRemoteStreamContent> GetFileAsync(GetFileInput input);

        Task<AppFileDescriptorDto> UploadFileAsync(IRemoteStreamContent input);

        Task<PagedResultDto<SpecialAdvertismentWithNavigationPropertiesDto>> GetListAsync(GetSpecialAdvertismentsInput input);

        Task<SpecialAdvertismentWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<SpecialAdvertismentDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetSitePropertyLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<SpecialAdvertismentDto> CreateAsync(SpecialAdvertismentCreateDto input);

        Task<SpecialAdvertismentDto> UpdateAsync(Guid id, SpecialAdvertismentUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(SpecialAdvertismentExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> specialadvertismentIds);

        Task DeleteAllAsync(GetSpecialAdvertismentsInput input);
        Task<AhlanFeekumPro.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}