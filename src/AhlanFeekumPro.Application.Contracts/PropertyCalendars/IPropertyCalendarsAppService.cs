using AhlanFeekumPro.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using AhlanFeekumPro.Shared;

namespace AhlanFeekumPro.PropertyCalendars
{
    public partial interface IPropertyCalendarsAppService : IApplicationService
    {

        Task<PagedResultDto<PropertyCalendarWithNavigationPropertiesDto>> GetListAsync(GetPropertyCalendarsInput input);

        Task<PropertyCalendarWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<PropertyCalendarDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetSitePropertyLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<PropertyCalendarDto> CreateAsync(PropertyCalendarCreateDto input);

        Task<PropertyCalendarDto> UpdateAsync(Guid id, PropertyCalendarUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(PropertyCalendarExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> propertycalendarIds);

        Task DeleteAllAsync(GetPropertyCalendarsInput input);
        Task<AhlanFeekumPro.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

    }
}