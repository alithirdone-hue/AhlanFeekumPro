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
using AhlanFeekumPro.PropertyFeatures;
using AhlanFeekumPro.Permissions;
using AhlanFeekumPro.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace AhlanFeekumPro.Blazor.Pages
{
    public partial class PropertyFeatures
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<PropertyFeatureDto> PropertyFeatureList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreatePropertyFeature { get; set; }
        private bool CanEditPropertyFeature { get; set; }
        private bool CanDeletePropertyFeature { get; set; }
        private PropertyFeatureCreateDto NewPropertyFeature { get; set; }
        private Validations NewPropertyFeatureValidations { get; set; } = new();
        private PropertyFeatureUpdateDto EditingPropertyFeature { get; set; }
        private Validations EditingPropertyFeatureValidations { get; set; } = new();
        private Guid EditingPropertyFeatureId { get; set; }
        private Modal CreatePropertyFeatureModal { get; set; } = new();
        private Modal EditPropertyFeatureModal { get; set; } = new();
        private GetPropertyFeaturesInput Filter { get; set; }
        private DataGridEntityActionsColumn<PropertyFeatureDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "propertyFeature-create-tab";
        protected string SelectedEditTab = "propertyFeature-edit-tab";
        private PropertyFeatureDto? SelectedPropertyFeature;
        
        
        
        
        
        private List<PropertyFeatureDto> SelectedPropertyFeatures { get; set; } = new();
        private bool AllPropertyFeaturesSelected { get; set; }
        
        public PropertyFeatures()
        {
            NewPropertyFeature = new PropertyFeatureCreateDto();
            EditingPropertyFeature = new PropertyFeatureUpdateDto();
            Filter = new GetPropertyFeaturesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            PropertyFeatureList = new List<PropertyFeatureDto>();
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["PropertyFeatures"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewPropertyFeature"], async () =>
            {
                await OpenCreatePropertyFeatureModalAsync();
            }, IconName.Add, requiredPolicyName: AhlanFeekumProPermissions.PropertyFeatures.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreatePropertyFeature = await AuthorizationService
                .IsGrantedAsync(AhlanFeekumProPermissions.PropertyFeatures.Create);
            CanEditPropertyFeature = await AuthorizationService
                            .IsGrantedAsync(AhlanFeekumProPermissions.PropertyFeatures.Edit);
            CanDeletePropertyFeature = await AuthorizationService
                            .IsGrantedAsync(AhlanFeekumProPermissions.PropertyFeatures.Delete);
                            
                            
        }

        private async Task GetPropertyFeaturesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await PropertyFeaturesAppService.GetListAsync(Filter);
            PropertyFeatureList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetPropertyFeaturesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await PropertyFeaturesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("AhlanFeekumPro") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/property-features/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Title={HttpUtility.UrlEncode(Filter.Title)}&Icon={HttpUtility.UrlEncode(Filter.Icon)}&OrderMin={Filter.OrderMin}&OrderMax={Filter.OrderMax}&IsActive={Filter.IsActive}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<PropertyFeatureDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetPropertyFeaturesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreatePropertyFeatureModalAsync()
        {
            NewPropertyFeature = new PropertyFeatureCreateDto{
                
                
            };

            SelectedCreateTab = "propertyFeature-create-tab";
            
            
            await NewPropertyFeatureValidations.ClearAll();
            await CreatePropertyFeatureModal.Show();
        }

        private async Task CloseCreatePropertyFeatureModalAsync()
        {
            NewPropertyFeature = new PropertyFeatureCreateDto{
                
                
            };
            await CreatePropertyFeatureModal.Hide();
        }

        private async Task OpenEditPropertyFeatureModalAsync(PropertyFeatureDto input)
        {
            SelectedEditTab = "propertyFeature-edit-tab";
            
            
            var propertyFeature = await PropertyFeaturesAppService.GetAsync(input.Id);
            
            EditingPropertyFeatureId = propertyFeature.Id;
            EditingPropertyFeature = ObjectMapper.Map<PropertyFeatureDto, PropertyFeatureUpdateDto>(propertyFeature);
            
            await EditingPropertyFeatureValidations.ClearAll();
            await EditPropertyFeatureModal.Show();
        }

        private async Task DeletePropertyFeatureAsync(PropertyFeatureDto input)
        {
            await PropertyFeaturesAppService.DeleteAsync(input.Id);
            await GetPropertyFeaturesAsync();
        }

        private async Task CreatePropertyFeatureAsync()
        {
            try
            {
                if (await NewPropertyFeatureValidations.ValidateAll() == false)
                {
                    return;
                }

                await PropertyFeaturesAppService.CreateAsync(NewPropertyFeature);
                await GetPropertyFeaturesAsync();
                await CloseCreatePropertyFeatureModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditPropertyFeatureModalAsync()
        {
            await EditPropertyFeatureModal.Hide();
        }

        private async Task UpdatePropertyFeatureAsync()
        {
            try
            {
                if (await EditingPropertyFeatureValidations.ValidateAll() == false)
                {
                    return;
                }

                await PropertyFeaturesAppService.UpdateAsync(EditingPropertyFeatureId, EditingPropertyFeature);
                await GetPropertyFeaturesAsync();
                await EditPropertyFeatureModal.Hide();                
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









        protected virtual async Task OnTitleChangedAsync(string? title)
        {
            Filter.Title = title;
            await SearchAsync();
        }
        protected virtual async Task OnIconChangedAsync(string? icon)
        {
            Filter.Icon = icon;
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
        protected virtual async Task OnIsActiveChangedAsync(bool? isActive)
        {
            Filter.IsActive = isActive;
            await SearchAsync();
        }
        





        private Task SelectAllItems()
        {
            AllPropertyFeaturesSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllPropertyFeaturesSelected = false;
            SelectedPropertyFeatures.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedPropertyFeatureRowsChanged()
        {
            if (SelectedPropertyFeatures.Count != PageSize)
            {
                AllPropertyFeaturesSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedPropertyFeaturesAsync()
        {
            var message = AllPropertyFeaturesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedPropertyFeatures.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllPropertyFeaturesSelected)
            {
                await PropertyFeaturesAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await PropertyFeaturesAppService.DeleteByIdsAsync(SelectedPropertyFeatures.Select(x => x.Id).ToList());
            }

            SelectedPropertyFeatures.Clear();
            AllPropertyFeaturesSelected = false;

            await GetPropertyFeaturesAsync();
        }


    }
}
