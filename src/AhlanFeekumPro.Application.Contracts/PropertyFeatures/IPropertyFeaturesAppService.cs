using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using AhlanFeekumPro.Shared;

namespace AhlanFeekumPro.PropertyFeatures
{
    public partial interface IPropertyFeaturesAppService : IApplicationService
    {

        Task<PagedResultDto<PropertyFeatureDto>> GetListAsync(GetPropertyFeaturesInput input);

        Task<PropertyFeatureDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<PropertyFeatureDto> CreateAsync(PropertyFeatureCreateDto input);

        Task<PropertyFeatureDto> UpdateAsync(Guid id, PropertyFeatureUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(PropertyFeatureExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> propertyfeatureIds);

        Task DeleteAllAsync(GetPropertyFeaturesInput input);
        Task<AhlanFeekumPro.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}