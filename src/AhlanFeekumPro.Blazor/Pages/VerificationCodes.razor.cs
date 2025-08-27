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
using AhlanFeekumPro.VerificationCodes;
using AhlanFeekumPro.Permissions;
using AhlanFeekumPro.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace AhlanFeekumPro.Blazor.Pages
{
    public partial class VerificationCodes
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<VerificationCodeDto> VerificationCodeList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateVerificationCode { get; set; }
        private bool CanEditVerificationCode { get; set; }
        private bool CanDeleteVerificationCode { get; set; }
        private VerificationCodeCreateDto NewVerificationCode { get; set; }
        private Validations NewVerificationCodeValidations { get; set; } = new();
        private VerificationCodeUpdateDto EditingVerificationCode { get; set; }
        private Validations EditingVerificationCodeValidations { get; set; } = new();
        private Guid EditingVerificationCodeId { get; set; }
        private Modal CreateVerificationCodeModal { get; set; } = new();
        private Modal EditVerificationCodeModal { get; set; } = new();
        private GetVerificationCodesInput Filter { get; set; }
        private DataGridEntityActionsColumn<VerificationCodeDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "verificationCode-create-tab";
        protected string SelectedEditTab = "verificationCode-edit-tab";
        private VerificationCodeDto? SelectedVerificationCode;
        
        
        
        
        
        private List<VerificationCodeDto> SelectedVerificationCodes { get; set; } = new();
        private bool AllVerificationCodesSelected { get; set; }
        
        public VerificationCodes()
        {
            NewVerificationCode = new VerificationCodeCreateDto();
            EditingVerificationCode = new VerificationCodeUpdateDto();
            Filter = new GetVerificationCodesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            VerificationCodeList = new List<VerificationCodeDto>();
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["VerificationCodes"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewVerificationCode"], async () =>
            {
                await OpenCreateVerificationCodeModalAsync();
            }, IconName.Add, requiredPolicyName: AhlanFeekumProPermissions.VerificationCodes.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateVerificationCode = await AuthorizationService
                .IsGrantedAsync(AhlanFeekumProPermissions.VerificationCodes.Create);
            CanEditVerificationCode = await AuthorizationService
                            .IsGrantedAsync(AhlanFeekumProPermissions.VerificationCodes.Edit);
            CanDeleteVerificationCode = await AuthorizationService
                            .IsGrantedAsync(AhlanFeekumProPermissions.VerificationCodes.Delete);
                            
                            
        }

        private async Task GetVerificationCodesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await VerificationCodesAppService.GetListAsync(Filter);
            VerificationCodeList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetVerificationCodesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await VerificationCodesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("AhlanFeekumPro") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/verification-codes/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&PhoneOrEmail={HttpUtility.UrlEncode(Filter.PhoneOrEmail)}&SecurityCodeMin={Filter.SecurityCodeMin}&SecurityCodeMax={Filter.SecurityCodeMax}&IsExpired={Filter.IsExpired}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<VerificationCodeDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetVerificationCodesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateVerificationCodeModalAsync()
        {
            NewVerificationCode = new VerificationCodeCreateDto{
                
                
            };

            SelectedCreateTab = "verificationCode-create-tab";
            
            
            await NewVerificationCodeValidations.ClearAll();
            await CreateVerificationCodeModal.Show();
        }

        private async Task CloseCreateVerificationCodeModalAsync()
        {
            NewVerificationCode = new VerificationCodeCreateDto{
                
                
            };
            await CreateVerificationCodeModal.Hide();
        }

        private async Task OpenEditVerificationCodeModalAsync(VerificationCodeDto input)
        {
            SelectedEditTab = "verificationCode-edit-tab";
            
            
            var verificationCode = await VerificationCodesAppService.GetAsync(input.Id);
            
            EditingVerificationCodeId = verificationCode.Id;
            EditingVerificationCode = ObjectMapper.Map<VerificationCodeDto, VerificationCodeUpdateDto>(verificationCode);
            
            await EditingVerificationCodeValidations.ClearAll();
            await EditVerificationCodeModal.Show();
        }

        private async Task DeleteVerificationCodeAsync(VerificationCodeDto input)
        {
            await VerificationCodesAppService.DeleteAsync(input.Id);
            await GetVerificationCodesAsync();
        }

        private async Task CreateVerificationCodeAsync()
        {
            try
            {
                if (await NewVerificationCodeValidations.ValidateAll() == false)
                {
                    return;
                }

                await VerificationCodesAppService.CreateAsync(NewVerificationCode);
                await GetVerificationCodesAsync();
                await CloseCreateVerificationCodeModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditVerificationCodeModalAsync()
        {
            await EditVerificationCodeModal.Hide();
        }

        private async Task UpdateVerificationCodeAsync()
        {
            try
            {
                if (await EditingVerificationCodeValidations.ValidateAll() == false)
                {
                    return;
                }

                await VerificationCodesAppService.UpdateAsync(EditingVerificationCodeId, EditingVerificationCode);
                await GetVerificationCodesAsync();
                await EditVerificationCodeModal.Hide();                
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









        protected virtual async Task OnPhoneOrEmailChangedAsync(string? phoneOrEmail)
        {
            Filter.PhoneOrEmail = phoneOrEmail;
            await SearchAsync();
        }
        protected virtual async Task OnSecurityCodeMinChangedAsync(int? securityCodeMin)
        {
            Filter.SecurityCodeMin = securityCodeMin;
            await SearchAsync();
        }
        protected virtual async Task OnSecurityCodeMaxChangedAsync(int? securityCodeMax)
        {
            Filter.SecurityCodeMax = securityCodeMax;
            await SearchAsync();
        }
        protected virtual async Task OnIsExpiredChangedAsync(bool? isExpired)
        {
            Filter.IsExpired = isExpired;
            await SearchAsync();
        }
        





        private Task SelectAllItems()
        {
            AllVerificationCodesSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllVerificationCodesSelected = false;
            SelectedVerificationCodes.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedVerificationCodeRowsChanged()
        {
            if (SelectedVerificationCodes.Count != PageSize)
            {
                AllVerificationCodesSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedVerificationCodesAsync()
        {
            var message = AllVerificationCodesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedVerificationCodes.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllVerificationCodesSelected)
            {
                await VerificationCodesAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await VerificationCodesAppService.DeleteByIdsAsync(SelectedVerificationCodes.Select(x => x.Id).ToList());
            }

            SelectedVerificationCodes.Clear();
            AllVerificationCodesSelected = false;

            await GetVerificationCodesAsync();
        }


    }
}
