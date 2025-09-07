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
using AhlanFeekumPro.Governorates;
using AhlanFeekumPro.Permissions;
using AhlanFeekumPro.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace AhlanFeekumPro.Blazor.Pages
{
    public partial class Governorates
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<GovernorateDto> GovernorateList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateGovernorate { get; set; }
        private bool CanEditGovernorate { get; set; }
        private bool CanDeleteGovernorate { get; set; }
        private GovernorateCreateDto NewGovernorate { get; set; }
        private Validations NewGovernorateValidations { get; set; } = new();
        private GovernorateUpdateDto EditingGovernorate { get; set; }
        private Validations EditingGovernorateValidations { get; set; } = new();
        private Guid EditingGovernorateId { get; set; }
        private Modal CreateGovernorateModal { get; set; } = new();
        private Modal EditGovernorateModal { get; set; } = new();
        private GetGovernoratesInput Filter { get; set; }
        private DataGridEntityActionsColumn<GovernorateDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "governorate-create-tab";
        protected string SelectedEditTab = "governorate-edit-tab";
        private GovernorateDto? SelectedGovernorate;
        
        
        
        
        
        private List<GovernorateDto> SelectedGovernorates { get; set; } = new();
        private bool AllGovernoratesSelected { get; set; }
        
        public Governorates()
        {
            NewGovernorate = new GovernorateCreateDto();
            EditingGovernorate = new GovernorateUpdateDto();
            Filter = new GetGovernoratesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            GovernorateList = new List<GovernorateDto>();
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Governorates"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewGovernorate"], async () =>
            {
                await OpenCreateGovernorateModalAsync();
            }, IconName.Add, requiredPolicyName: AhlanFeekumProPermissions.Governorates.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateGovernorate = await AuthorizationService
                .IsGrantedAsync(AhlanFeekumProPermissions.Governorates.Create);
            CanEditGovernorate = await AuthorizationService
                            .IsGrantedAsync(AhlanFeekumProPermissions.Governorates.Edit);
            CanDeleteGovernorate = await AuthorizationService
                            .IsGrantedAsync(AhlanFeekumProPermissions.Governorates.Delete);
                            
                            
        }

        private async Task GetGovernoratesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await GovernoratesAppService.GetListAsync(Filter);
            GovernorateList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetGovernoratesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await GovernoratesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("AhlanFeekumPro") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/governorates/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Title={HttpUtility.UrlEncode(Filter.Title)}&OrderMin={Filter.OrderMin}&OrderMax={Filter.OrderMax}&IsActive={Filter.IsActive}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<GovernorateDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetGovernoratesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateGovernorateModalAsync()
        {
            NewGovernorate = new GovernorateCreateDto{
                
                
            };

            SelectedCreateTab = "governorate-create-tab";
            
            
            await NewGovernorateValidations.ClearAll();
            await CreateGovernorateModal.Show();
        }

        private async Task CloseCreateGovernorateModalAsync()
        {
            NewGovernorate = new GovernorateCreateDto{
                
                
            };
            await CreateGovernorateModal.Hide();
        }

        private async Task OpenEditGovernorateModalAsync(GovernorateDto input)
        {
            SelectedEditTab = "governorate-edit-tab";
            
            
            var governorate = await GovernoratesAppService.GetAsync(input.Id);
            
            EditingGovernorateId = governorate.Id;
            EditingGovernorate = ObjectMapper.Map<GovernorateDto, GovernorateUpdateDto>(governorate);
            
            await EditingGovernorateValidations.ClearAll();
            await EditGovernorateModal.Show();
        }

        private async Task DeleteGovernorateAsync(GovernorateDto input)
        {
            await GovernoratesAppService.DeleteAsync(input.Id);
            await GetGovernoratesAsync();
        }

        private async Task CreateGovernorateAsync()
        {
            try
            {
                if (await NewGovernorateValidations.ValidateAll() == false)
                {
                    return;
                }

                await GovernoratesAppService.CreateAsync(NewGovernorate);
                await GetGovernoratesAsync();
                await CloseCreateGovernorateModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditGovernorateModalAsync()
        {
            await EditGovernorateModal.Hide();
        }

        private async Task UpdateGovernorateAsync()
        {
            try
            {
                if (await EditingGovernorateValidations.ValidateAll() == false)
                {
                    return;
                }

                await GovernoratesAppService.UpdateAsync(EditingGovernorateId, EditingGovernorate);
                await GetGovernoratesAsync();
                await EditGovernorateModal.Hide();                
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
            AllGovernoratesSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllGovernoratesSelected = false;
            SelectedGovernorates.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedGovernorateRowsChanged()
        {
            if (SelectedGovernorates.Count != PageSize)
            {
                AllGovernoratesSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedGovernoratesAsync()
        {
            var message = AllGovernoratesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedGovernorates.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllGovernoratesSelected)
            {
                await GovernoratesAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await GovernoratesAppService.DeleteByIdsAsync(SelectedGovernorates.Select(x => x.Id).ToList());
            }

            SelectedGovernorates.Clear();
            AllGovernoratesSelected = false;

            await GetGovernoratesAsync();
        }


    }
}
