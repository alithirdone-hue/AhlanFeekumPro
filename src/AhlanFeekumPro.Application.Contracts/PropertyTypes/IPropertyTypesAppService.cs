using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using AhlanFeekumPro.Shared;

namespace AhlanFeekumPro.PropertyTypes
{
    public partial interface IPropertyTypesAppService : IApplicationService
    {

        Task<PagedResultDto<PropertyTypeDto>> GetListAsync(GetPropertyTypesInput input);

        Task<PropertyTypeDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<PropertyTypeDto> CreateAsync(PropertyTypeCreateDto input);

        Task<PropertyTypeDto> UpdateAsync(Guid id, PropertyTypeUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(PropertyTypeExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> propertytypeIds);

        Task DeleteAllAsync(GetPropertyTypesInput input);
        Task<AhlanFeekumPro.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}