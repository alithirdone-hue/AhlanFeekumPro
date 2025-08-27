using AhlanFeekumPro.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using AhlanFeekumPro.Shared;

namespace AhlanFeekumPro.PersonEvaluations
{
    public partial interface IPersonEvaluationsAppService : IApplicationService
    {

        Task<PagedResultDto<PersonEvaluationWithNavigationPropertiesDto>> GetListAsync(GetPersonEvaluationsInput input);

        Task<PersonEvaluationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<PersonEvaluationDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<PersonEvaluationDto> CreateAsync(PersonEvaluationCreateDto input);

        Task<PersonEvaluationDto> UpdateAsync(Guid id, PersonEvaluationUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(PersonEvaluationExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> personevaluationIds);

        Task DeleteAllAsync(GetPersonEvaluationsInput input);
        Task<AhlanFeekumPro.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}