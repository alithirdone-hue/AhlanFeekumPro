using AhlanFeekumPro.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using AhlanFeekumPro.Shared;

namespace AhlanFeekumPro.SiteProperties
{
    public partial interface ISitePropertiesAppService : IApplicationService
    {

        Task<PagedResultDto<SitePropertyWithNavigationPropertiesDto>> GetListAsync(GetSitePropertiesInput input);

        Task<SitePropertyWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<SitePropertyDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetPropertyTypeLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetGovernorateLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetPropertyFeatureLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<SitePropertyDto> CreateAsync(SitePropertyCreateDto input);

        Task<SitePropertyDto> UpdateAsync(Guid id, SitePropertyUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(SitePropertyExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> sitepropertyIds);

        Task DeleteAllAsync(GetSitePropertiesInput input);
        Task<AhlanFeekumPro.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}