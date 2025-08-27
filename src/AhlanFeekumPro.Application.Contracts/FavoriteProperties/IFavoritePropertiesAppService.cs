using AhlanFeekumPro.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using AhlanFeekumPro.Shared;

namespace AhlanFeekumPro.FavoriteProperties
{
    public partial interface IFavoritePropertiesAppService : IApplicationService
    {

        Task<PagedResultDto<FavoritePropertyWithNavigationPropertiesDto>> GetListAsync(GetFavoritePropertiesInput input);

        Task<FavoritePropertyWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<FavoritePropertyDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetUserProfileLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetSitePropertyLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<FavoritePropertyDto> CreateAsync(FavoritePropertyCreateDto input);

        Task<FavoritePropertyDto> UpdateAsync(Guid id, FavoritePropertyUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(FavoritePropertyExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> favoritepropertyIds);

        Task DeleteAllAsync(GetFavoritePropertiesInput input);
        Task<AhlanFeekumPro.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}