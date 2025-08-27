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
using AhlanFeekumPro.PropertyTypes;
using AhlanFeekumPro.Permissions;
using AhlanFeekumPro.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace AhlanFeekumPro.Blazor.Pages
{
    public partial class PropertyTypes
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<PropertyTypeDto> PropertyTypeList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreatePropertyType { get; set; }
        private bool CanEditPropertyType { get; set; }
        private bool CanDeletePropertyType { get; set; }
        private PropertyTypeCreateDto NewPropertyType { get; set; }
        private Validations NewPropertyTypeValidations { get; set; } = new();
        private PropertyTypeUpdateDto EditingPropertyType { get; set; }
        private Validations EditingPropertyTypeValidations { get; set; } = new();
        private Guid EditingPropertyTypeId { get; set; }
        private Modal CreatePropertyTypeModal { get; set; } = new();
        private Modal EditPropertyTypeModal { get; set; } = new();
        private GetPropertyTypesInput Filter { get; set; }
        private DataGridEntityActionsColumn<PropertyTypeDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "propertyType-create-tab";
        protected string SelectedEditTab = "propertyType-edit-tab";
        private PropertyTypeDto? SelectedPropertyType;
        
        
        
        
        
        private List<PropertyTypeDto> SelectedPropertyTypes { get; set; } = new();
        private bool AllPropertyTypesSelected { get; set; }
        
        public PropertyTypes()
        {
            NewPropertyType = new PropertyTypeCreateDto();
            EditingPropertyType = new PropertyTypeUpdateDto();
            Filter = new GetPropertyTypesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            PropertyTypeList = new List<PropertyTypeDto>();
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["PropertyTypes"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewPropertyType"], async () =>
            {
                await OpenCreatePropertyTypeModalAsync();
            }, IconName.Add, requiredPolicyName: AhlanFeekumProPermissions.PropertyTypes.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreatePropertyType = await AuthorizationService
                .IsGrantedAsync(AhlanFeekumProPermissions.PropertyTypes.Create);
            CanEditPropertyType = await AuthorizationService
                            .IsGrantedAsync(AhlanFeekumProPermissions.PropertyTypes.Edit);
            CanDeletePropertyType = await AuthorizationService
                            .IsGrantedAsync(AhlanFeekumProPermissions.PropertyTypes.Delete);
                            
                            
        }

        private async Task GetPropertyTypesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await PropertyTypesAppService.GetListAsync(Filter);
            PropertyTypeList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetPropertyTypesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await PropertyTypesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("AhlanFeekumPro") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/property-types/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Title={HttpUtility.UrlEncode(Filter.Title)}&OrderMin={Filter.OrderMin}&OrderMax={Filter.OrderMax}&IsActive={Filter.IsActive}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<PropertyTypeDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetPropertyTypesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreatePropertyTypeModalAsync()
        {
            NewPropertyType = new PropertyTypeCreateDto{
                
                
            };

            SelectedCreateTab = "propertyType-create-tab";
            
            
            await NewPropertyTypeValidations.ClearAll();
            await CreatePropertyTypeModal.Show();
        }

        private async Task CloseCreatePropertyTypeModalAsync()
        {
            NewPropertyType = new PropertyTypeCreateDto{
                
                
            };
            await CreatePropertyTypeModal.Hide();
        }

        private async Task OpenEditPropertyTypeModalAsync(PropertyTypeDto input)
        {
            SelectedEditTab = "propertyType-edit-tab";
            
            
            var propertyType = await PropertyTypesAppService.GetAsync(input.Id);
            
            EditingPropertyTypeId = propertyType.Id;
            EditingPropertyType = ObjectMapper.Map<PropertyTypeDto, PropertyTypeUpdateDto>(propertyType);
            
            await EditingPropertyTypeValidations.ClearAll();
            await EditPropertyTypeModal.Show();
        }

        private async Task DeletePropertyTypeAsync(PropertyTypeDto input)
        {
            await PropertyTypesAppService.DeleteAsync(input.Id);
            await GetPropertyTypesAsync();
        }

        private async Task CreatePropertyTypeAsync()
        {
            try
            {
                if (await NewPropertyTypeValidations.ValidateAll() == false)
                {
                    return;
                }

                await PropertyTypesAppService.CreateAsync(NewPropertyType);
                await GetPropertyTypesAsync();
                await CloseCreatePropertyTypeModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditPropertyTypeModalAsync()
        {
            await EditPropertyTypeModal.Hide();
        }

        private async Task UpdatePropertyTypeAsync()
        {
            try
            {
                if (await EditingPropertyTypeValidations.ValidateAll() == false)
                {
                    return;
                }

                await PropertyTypesAppService.UpdateAsync(EditingPropertyTypeId, EditingPropertyType);
                await GetPropertyTypesAsync();
                await EditPropertyTypeModal.Hide();                
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
            AllPropertyTypesSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllPropertyTypesSelected = false;
            SelectedPropertyTypes.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedPropertyTypeRowsChanged()
        {
            if (SelectedPropertyTypes.Count != PageSize)
            {
                AllPropertyTypesSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedPropertyTypesAsync()
        {
            var message = AllPropertyTypesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedPropertyTypes.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllPropertyTypesSelected)
            {
                await PropertyTypesAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await PropertyTypesAppService.DeleteByIdsAsync(SelectedPropertyTypes.Select(x => x.Id).ToList());
            }

            SelectedPropertyTypes.Clear();
            AllPropertyTypesSelected = false;

            await GetPropertyTypesAsync();
        }


    }
}
