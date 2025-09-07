using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using AhlanFeekumPro.Shared;

namespace AhlanFeekumPro.OnlyForYouSections
{
    public partial interface IOnlyForYouSectionsAppService : IApplicationService
    {

        Task<PagedResultDto<OnlyForYouSectionDto>> GetListAsync(GetOnlyForYouSectionsInput input);

        Task<OnlyForYouSectionDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<OnlyForYouSectionDto> CreateAsync(OnlyForYouSectionCreateDto input);

        Task<OnlyForYouSectionDto> UpdateAsync(Guid id, OnlyForYouSectionUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(OnlyForYouSectionExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> onlyforyousectionIds);

        Task DeleteAllAsync(GetOnlyForYouSectionsInput input);
        Task<AhlanFeekumPro.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}