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
using AhlanFeekumPro.PropertyMedias;
using AhlanFeekumPro.Permissions;
using AhlanFeekumPro.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace AhlanFeekumPro.Blazor.Pages
{
    public partial class PropertyMedias
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<PropertyMediaWithNavigationPropertiesDto> PropertyMediaList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreatePropertyMedia { get; set; }
        private bool CanEditPropertyMedia { get; set; }
        private bool CanDeletePropertyMedia { get; set; }
        private PropertyMediaCreateDto NewPropertyMedia { get; set; }
        private Validations NewPropertyMediaValidations { get; set; } = new();
        private PropertyMediaUpdateDto EditingPropertyMedia { get; set; }
        private Validations EditingPropertyMediaValidations { get; set; } = new();
        private Guid EditingPropertyMediaId { get; set; }
        private Modal CreatePropertyMediaModal { get; set; } = new();
        private Modal EditPropertyMediaModal { get; set; } = new();
        private GetPropertyMediasInput Filter { get; set; }
        private DataGridEntityActionsColumn<PropertyMediaWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "propertyMedia-create-tab";
        protected string SelectedEditTab = "propertyMedia-edit-tab";
        private PropertyMediaWithNavigationPropertiesDto? SelectedPropertyMedia;
        private IReadOnlyList<LookupDto<Guid>> SitePropertiesCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        private List<PropertyMediaWithNavigationPropertiesDto> SelectedPropertyMedias { get; set; } = new();
        private bool AllPropertyMediasSelected { get; set; }
        
        public PropertyMedias()
        {
            NewPropertyMedia = new PropertyMediaCreateDto();
            EditingPropertyMedia = new PropertyMediaUpdateDto();
            Filter = new GetPropertyMediasInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            PropertyMediaList = new List<PropertyMediaWithNavigationPropertiesDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["PropertyMedias"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewPropertyMedia"], async () =>
            {
                await OpenCreatePropertyMediaModalAsync();
            }, IconName.Add, requiredPolicyName: AhlanFeekumProPermissions.PropertyMedias.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreatePropertyMedia = await AuthorizationService
                .IsGrantedAsync(AhlanFeekumProPermissions.PropertyMedias.Create);
            CanEditPropertyMedia = await AuthorizationService
                            .IsGrantedAsync(AhlanFeekumProPermissions.PropertyMedias.Edit);
            CanDeletePropertyMedia = await AuthorizationService
                            .IsGrantedAsync(AhlanFeekumProPermissions.PropertyMedias.Delete);
                            
                            
        }

        private async Task GetPropertyMediasAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await PropertyMediasAppService.GetListAsync(Filter);
            PropertyMediaList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetPropertyMediasAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await PropertyMediasAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("AhlanFeekumPro") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/property-medias/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Image={HttpUtility.UrlEncode(Filter.Image)}&OrderMin={Filter.OrderMin}&OrderMax={Filter.OrderMax}&isActive={Filter.isActive}&SitePropertyId={Filter.SitePropertyId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<PropertyMediaWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetPropertyMediasAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreatePropertyMediaModalAsync()
        {
            NewPropertyMedia = new PropertyMediaCreateDto{
                
                
            };

            SelectedCreateTab = "propertyMedia-create-tab";
            
            
            await NewPropertyMediaValidations.ClearAll();
            await CreatePropertyMediaModal.Show();
        }

        private async Task CloseCreatePropertyMediaModalAsync()
        {
            NewPropertyMedia = new PropertyMediaCreateDto{
                
                
            };
            await CreatePropertyMediaModal.Hide();
        }

        private async Task OpenEditPropertyMediaModalAsync(PropertyMediaWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "propertyMedia-edit-tab";
            
            
            var propertyMedia = await PropertyMediasAppService.GetWithNavigationPropertiesAsync(input.PropertyMedia.Id);
            
            EditingPropertyMediaId = propertyMedia.PropertyMedia.Id;
            EditingPropertyMedia = ObjectMapper.Map<PropertyMediaDto, PropertyMediaUpdateDto>(propertyMedia.PropertyMedia);
            
            await EditingPropertyMediaValidations.ClearAll();
            await EditPropertyMediaModal.Show();
        }

        private async Task DeletePropertyMediaAsync(PropertyMediaWithNavigationPropertiesDto input)
        {
            await PropertyMediasAppService.DeleteAsync(input.PropertyMedia.Id);
            await GetPropertyMediasAsync();
        }

        private async Task CreatePropertyMediaAsync()
        {
            try
            {
                if (await NewPropertyMediaValidations.ValidateAll() == false)
                {
                    return;
                }

                await PropertyMediasAppService.CreateAsync(NewPropertyMedia);
                await GetPropertyMediasAsync();
                await CloseCreatePropertyMediaModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditPropertyMediaModalAsync()
        {
            await EditPropertyMediaModal.Hide();
        }

        private async Task UpdatePropertyMediaAsync()
        {
            try
            {
                if (await EditingPropertyMediaValidations.ValidateAll() == false)
                {
                    return;
                }

                await PropertyMediasAppService.UpdateAsync(EditingPropertyMediaId, EditingPropertyMedia);
                await GetPropertyMediasAsync();
                await EditPropertyMediaModal.Hide();                
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









        protected virtual async Task OnImageChangedAsync(string? image)
        {
            Filter.Image = image;
            await SearchAsync();
        }
        protected virtual async Task OnOrderMinChangedAsync(int? orderMin)
        {
            Filter.OrderMin = orderMin;
            await SearchAsync();
        }
        protected virtual async Task OnOrderMaxChangedAsync(int? orderMax)
        {
            Filter.OrderMax = orderMax;
            await SearchAsync();
        }
        protected virtual async Task OnisActiveChangedAsync(bool? isActive)
        {
            Filter.isActive = isActive;
            await SearchAsync();
        }
        protected virtual async Task OnSitePropertyIdChangedAsync(Guid? sitePropertyId)
        {
            Filter.SitePropertyId = sitePropertyId;
            await SearchAsync();
        }
        

        private async Task GetSitePropertyCollectionLookupAsync(string? newValue = null)
        {
            SitePropertiesCollection = (await PropertyMediasAppService.GetSitePropertyLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }





        private Task SelectAllItems()
        {
            AllPropertyMediasSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllPropertyMediasSelected = false;
            SelectedPropertyMedias.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedPropertyMediaRowsChanged()
        {
            if (SelectedPropertyMedias.Count != PageSize)
            {
                AllPropertyMediasSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedPropertyMediasAsync()
        {
            var message = AllPropertyMediasSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedPropertyMedias.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllPropertyMediasSelected)
            {
                await PropertyMediasAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await PropertyMediasAppService.DeleteByIdsAsync(SelectedPropertyMedias.Select(x => x.PropertyMedia.Id).ToList());
            }

            SelectedPropertyMedias.Clear();
            AllPropertyMediasSelected = false;

            await GetPropertyMediasAsync();
        }


    }
}
