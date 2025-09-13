using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.Web;
using Blazorise;
using Blazorise.DataGrid;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using AhlanFeekumPro.PropertyCalendars;
using AhlanFeekumPro.Permissions;
using AhlanFeekumPro.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace AhlanFeekumPro.Blazor.Pages
{
    public partial class PropertyCalendars
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<PropertyCalendarWithNavigationPropertiesDto> PropertyCalendarList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreatePropertyCalendar { get; set; }
        private bool CanEditPropertyCalendar { get; set; }
        private bool CanDeletePropertyCalendar { get; set; }
        private PropertyCalendarCreateDto NewPropertyCalendar { get; set; }
        private Validations NewPropertyCalendarValidations { get; set; } = new();
        private PropertyCalendarUpdateDto EditingPropertyCalendar { get; set; }
        private Validations EditingPropertyCalendarValidations { get; set; } = new();
        private Guid EditingPropertyCalendarId { get; set; }
        private Modal CreatePropertyCalendarModal { get; set; } = new();
        private Modal EditPropertyCalendarModal { get; set; } = new();
        private GetPropertyCalendarsInput Filter { get; set; }
        private DataGridEntityActionsColumn<PropertyCalendarWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "propertyCalendar-create-tab";
        protected string SelectedEditTab = "propertyCalendar-edit-tab";
        private PropertyCalendarWithNavigationPropertiesDto? SelectedPropertyCalendar;
        private IReadOnlyList<LookupDto<Guid>> SitePropertiesCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        private List<PropertyCalendarWithNavigationPropertiesDto> SelectedPropertyCalendars { get; set; } = new();
        private bool AllPropertyCalendarsSelected { get; set; }
        
        public PropertyCalendars()
        {
            NewPropertyCalendar = new PropertyCalendarCreateDto();
            EditingPropertyCalendar = new PropertyCalendarUpdateDto();
            Filter = new GetPropertyCalendarsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            PropertyCalendarList = new List<PropertyCalendarWithNavigationPropertiesDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetSitePropertyCollectionLookupAsync();


            
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                
                await SetBreadcrumbItemsAsync();
                await SetToolbarItemsAsync();
                await InvokeAsync(StateHasChanged);
            }
        }  

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["PropertyCalendars"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewPropertyCalendar"], async () =>
            {
                await OpenCreatePropertyCalendarModalAsync();
            }, IconName.Add, requiredPolicyName: AhlanFeekumProPermissions.PropertyCalendars.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreatePropertyCalendar = await AuthorizationService
                .IsGrantedAsync(AhlanFeekumProPermissions.PropertyCalendars.Create);
            CanEditPropertyCalendar = await AuthorizationService
                            .IsGrantedAsync(AhlanFeekumProPermissions.PropertyCalendars.Edit);
            CanDeletePropertyCalendar = await AuthorizationService
                            .IsGrantedAsync(AhlanFeekumProPermissions.PropertyCalendars.Delete);
                            
                            
        }

        private async Task GetPropertyCalendarsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await PropertyCalendarsAppService.GetListAsync(Filter);
            PropertyCalendarList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetPropertyCalendarsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await PropertyCalendarsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("AhlanFeekumPro") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/property-calendars/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&DateMin={Filter.DateMin}&DateMax={Filter.DateMax}&IsAvailable={Filter.IsAvailable}&PriceMin={Filter.PriceMin}&PriceMax={Filter.PriceMax}&Note={HttpUtility.UrlEncode(Filter.Note)}&SitePropertyId={Filter.SitePropertyId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<PropertyCalendarWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetPropertyCalendarsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreatePropertyCalendarModalAsync()
        {
            NewPropertyCalendar = new PropertyCalendarCreateDto{
                Date = DateOnly.FromDateTime(DateTime.Now),

                SitePropertyId = SitePropertiesCollection.Select(i=>i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "propertyCalendar-create-tab";
            
            
            await NewPropertyCalendarValidations.ClearAll();
            await CreatePropertyCalendarModal.Show();
        }

        private async Task CloseCreatePropertyCalendarModalAsync()
        {
            NewPropertyCalendar = new PropertyCalendarCreateDto{
                Date = DateOnly.FromDateTime(DateTime.Now),

                SitePropertyId = SitePropertiesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreatePropertyCalendarModal.Hide();
        }

        private async Task OpenEditPropertyCalendarModalAsync(PropertyCalendarWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "propertyCalendar-edit-tab";
            
            
            var propertyCalendar = await PropertyCalendarsAppService.GetWithNavigationPropertiesAsync(input.PropertyCalendar.Id);
            
            EditingPropertyCalendarId = propertyCalendar.PropertyCalendar.Id;
            EditingPropertyCalendar = ObjectMapper.Map<PropertyCalendarDto, PropertyCalendarUpdateDto>(propertyCalendar.PropertyCalendar);
            
            await EditingPropertyCalendarValidations.ClearAll();
            await EditPropertyCalendarModal.Show();
        }

        private async Task DeletePropertyCalendarAsync(PropertyCalendarWithNavigationPropertiesDto input)
        {
            await PropertyCalendarsAppService.DeleteAsync(input.PropertyCalendar.Id);
            await GetPropertyCalendarsAsync();
        }

        private async Task CreatePropertyCalendarAsync()
        {
            try
            {
                if (await NewPropertyCalendarValidations.ValidateAll() == false)
                {
                    return;
                }

                await PropertyCalendarsAppService.CreateAsync(NewPropertyCalendar);
                await GetPropertyCalendarsAsync();
                await CloseCreatePropertyCalendarModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditPropertyCalendarModalAsync()
        {
            await EditPropertyCalendarModal.Hide();
        }

        private async Task UpdatePropertyCalendarAsync()
        {
            try
            {
                if (await EditingPropertyCalendarValidations.ValidateAll() == false)
                {
                    return;
                }

                await PropertyCalendarsAppService.UpdateAsync(EditingPropertyCalendarId, EditingPropertyCalendar);
                await GetPropertyCalendarsAsync();
                await EditPropertyCalendarModal.Hide();                
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private void OnSelectedCreateTabChanged(string name)
        {
            SelectedCreateTab = name;
        }

        private void OnSelectedEditTabChanged(string name)
        {
            SelectedEditTab = name;
        }









        protected virtual async Task OnDateMinChangedAsync(DateOnly? dateMin)
        {
            Filter.DateMin = dateMin;
            await SearchAsync();
        }
        protected virtual async Task OnDateMaxChangedAsync(DateOnly? dateMax)
        {
            Filter.DateMax = dateMax;
            await SearchAsync();
        }
        protected virtual async Task OnIsAvailableChangedAsync(bool? isAvailable)
        {
            Filter.IsAvailable = isAvailable;
            await SearchAsync();
        }
        protected virtual async Task OnPriceMinChangedAsync(float? priceMin)
        {
            Filter.PriceMin = priceMin;
            await SearchAsync();
        }
        protected virtual async Task OnPriceMaxChangedAsync(float? priceMax)
        {
            Filter.PriceMax = priceMax;
            await SearchAsync();
        }
        protected virtual async Task OnNoteChangedAsync(string? note)
        {
            Filter.Note = note;
            await SearchAsync();
        }
        protected virtual async Task OnSitePropertyIdChangedAsync(Guid? sitePropertyId)
        {
            Filter.SitePropertyId = sitePropertyId;
            await SearchAsync();
        }
        

        private async Task GetSitePropertyCollectionLookupAsync(string? newValue = null)
        {
            SitePropertiesCollection = (await PropertyCalendarsAppService.GetSitePropertyLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }





        private Task SelectAllItems()
        {
            AllPropertyCalendarsSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllPropertyCalendarsSelected = false;
            SelectedPropertyCalendars.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedPropertyCalendarRowsChanged()
        {
            if (SelectedPropertyCalendars.Count != PageSize)
            {
                AllPropertyCalendarsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedPropertyCalendarsAsync()
        {
            var message = AllPropertyCalendarsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedPropertyCalendars.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllPropertyCalendarsSelected)
            {
                await PropertyCalendarsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await PropertyCalendarsAppService.DeleteByIdsAsync(SelectedPropertyCalendars.Select(x => x.PropertyCalendar.Id).ToList());
            }

            SelectedPropertyCalendars.Clear();
            AllPropertyCalendarsSelected = false;

            await GetPropertyCalendarsAsync();
        }


    }
}
