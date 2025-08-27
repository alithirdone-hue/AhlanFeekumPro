using AhlanFeekumPro.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using AhlanFeekumPro.Shared;

namespace AhlanFeekumPro.PropertyEvaluations
{
    public partial interface IPropertyEvaluationsAppService : IApplicationService
    {

        Task<PagedResultDto<PropertyEvaluationWithNavigationPropertiesDto>> GetListAsync(GetPropertyEvaluationsInput input);

        Task<PropertyEvaluationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<PropertyEvaluationDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetSitePropertyLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<PropertyEvaluationDto> CreateAsync(PropertyEvaluationCreateDto input);

        Task<PropertyEvaluationDto> UpdateAsync(Guid id, PropertyEvaluationUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(PropertyEvaluationExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> propertyevaluationIds);

        Task DeleteAllAsync(GetPropertyEvaluationsInput input);
        Task<AhlanFeekumPro.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}