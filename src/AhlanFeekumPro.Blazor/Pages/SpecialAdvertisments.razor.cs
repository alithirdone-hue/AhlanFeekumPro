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
using AhlanFeekumPro.SpecialAdvertisments;
using AhlanFeekumPro.Permissions;
using AhlanFeekumPro.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace AhlanFeekumPro.Blazor.Pages
{
    public partial class SpecialAdvertisments
    {
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }
            
        private IJSObjectReference? _jsObjectRef;
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<SpecialAdvertismentWithNavigationPropertiesDto> SpecialAdvertismentList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateSpecialAdvertisment { get; set; }
        private bool CanEditSpecialAdvertisment { get; set; }
        private bool CanDeleteSpecialAdvertisment { get; set; }
        private SpecialAdvertismentCreateDto NewSpecialAdvertisment { get; set; }
        private Validations NewSpecialAdvertismentValidations { get; set; } = new();
        private SpecialAdvertismentUpdateDto EditingSpecialAdvertisment { get; set; }
        private Validations EditingSpecialAdvertismentValidations { get; set; } = new();
        private Guid EditingSpecialAdvertismentId { get; set; }
        private Modal CreateSpecialAdvertismentModal { get; set; } = new();
        private Modal EditSpecialAdvertismentModal { get; set; } = new();
        private GetSpecialAdvertismentsInput Filter { get; set; }
        private DataGridEntityActionsColumn<SpecialAdvertismentWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "specialAdvertisment-create-tab";
        protected string SelectedEditTab = "specialAdvertisment-edit-tab";
        private SpecialAdvertismentWithNavigationPropertiesDto? SelectedSpecialAdvertisment;
        private IReadOnlyList<LookupDto<Guid>> SitePropertiesCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        private List<SpecialAdvertismentWithNavigationPropertiesDto> SelectedSpecialAdvertisments { get; set; } = new();
        private bool AllSpecialAdvertismentsSelected { get; set; }
        
        public SpecialAdvertisments()
        {
            NewSpecialAdvertisment = new SpecialAdvertismentCreateDto();
            EditingSpecialAdvertisment = new SpecialAdvertismentUpdateDto();
            Filter = new GetSpecialAdvertismentsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            SpecialAdvertismentList = new List<SpecialAdvertismentWithNavigationPropertiesDto>();
            
            
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
                _jsObjectRef = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/Pages/SpecialAdvertisments.razor.js");
                await SetBreadcrumbItemsAsync();
                await SetToolbarItemsAsync();
                await InvokeAsync(StateHasChanged);
            }
        }  

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["SpecialAdvertisments"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewSpecialAdvertisment"], async () =>
            {
                await OpenCreateSpecialAdvertismentModalAsync();
            }, IconName.Add, requiredPolicyName: AhlanFeekumProPermissions.SpecialAdvertisments.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateSpecialAdvertisment = await AuthorizationService
                .IsGrantedAsync(AhlanFeekumProPermissions.SpecialAdvertisments.Create);
            CanEditSpecialAdvertisment = await AuthorizationService
                            .IsGrantedAsync(AhlanFeekumProPermissions.SpecialAdvertisments.Edit);
            CanDeleteSpecialAdvertisment = await AuthorizationService
                            .IsGrantedAsync(AhlanFeekumProPermissions.SpecialAdvertisments.Delete);
                            
                            
        }

        private async Task GetSpecialAdvertismentsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await SpecialAdvertismentsAppService.GetListAsync(Filter);
            SpecialAdvertismentList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetSpecialAdvertismentsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await SpecialAdvertismentsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("AhlanFeekumPro") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/special-advertisments/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&OrderMin={Filter.OrderMin}&OrderMax={Filter.OrderMax}&IsActive={Filter.IsActive}&SitePropertyId={Filter.SitePropertyId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<SpecialAdvertismentWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetSpecialAdvertismentsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateSpecialAdvertismentModalAsync()
        {
            NewSpecialAdvertisment = new SpecialAdvertismentCreateDto{
                
                SitePropertyId = SitePropertiesCollection.Select(i=>i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "specialAdvertisment-create-tab";
            
            await _jsObjectRef!.InvokeVoidAsync("FileCleanup.clearInputFiles");
            await NewSpecialAdvertismentValidations.ClearAll();
            await CreateSpecialAdvertismentModal.Show();
        }

        private async Task CloseCreateSpecialAdvertismentModalAsync()
        {
            NewSpecialAdvertisment = new SpecialAdvertismentCreateDto{
                
                SitePropertyId = SitePropertiesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateSpecialAdvertismentModal.Hide();
        }

        private async Task OpenEditSpecialAdvertismentModalAsync(SpecialAdvertismentWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "specialAdvertisment-edit-tab";
            
            await _jsObjectRef!.InvokeVoidAsync("FileCleanup.clearInputFiles");
            var specialAdvertisment = await SpecialAdvertismentsAppService.GetWithNavigationPropertiesAsync(input.SpecialAdvertisment.Id);
            
            EditingSpecialAdvertismentId = specialAdvertisment.SpecialAdvertisment.Id;
            EditingSpecialAdvertisment = ObjectMapper.Map<SpecialAdvertismentDto, SpecialAdvertismentUpdateDto>(specialAdvertisment.SpecialAdvertisment);
            HasSelectedSpecialAdvertismentImage = EditingSpecialAdvertisment.ImageId != null && EditingSpecialAdvertisment.ImageId != Guid.Empty;

            await EditingSpecialAdvertismentValidations.ClearAll();
            await EditSpecialAdvertismentModal.Show();
        }

        private async Task DeleteSpecialAdvertismentAsync(SpecialAdvertismentWithNavigationPropertiesDto input)
        {
            await SpecialAdvertismentsAppService.DeleteAsync(input.SpecialAdvertisment.Id);
            await GetSpecialAdvertismentsAsync();
        }

        private async Task CreateSpecialAdvertismentAsync()
        {
            try
            {
                if (await NewSpecialAdvertismentValidations.ValidateAll() == false)
                {
                    return;
                }

                await SpecialAdvertismentsAppService.CreateAsync(NewSpecialAdvertisment);
                await GetSpecialAdvertismentsAsync();
                await CloseCreateSpecialAdvertismentModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditSpecialAdvertismentModalAsync()
        {
            await EditSpecialAdvertismentModal.Hide();
        }

        private async Task UpdateSpecialAdvertismentAsync()
        {
            try
            {
                if (await EditingSpecialAdvertismentValidations.ValidateAll() == false)
                {
                    return;
                }

                await SpecialAdvertismentsAppService.UpdateAsync(EditingSpecialAdvertismentId, EditingSpecialAdvertisment);
                await GetSpecialAdvertismentsAsync();
                await EditSpecialAdvertismentModal.Hide();                
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


        private bool IsCreateFormDisabled()
        {
            return OnNewSpecialAdvertismentImageLoading ||NewSpecialAdvertisment.ImageId == Guid.Empty ;
        }
        
        private bool IsEditFormDisabled()
        {
            return OnEditSpecialAdvertismentImageLoading ||EditingSpecialAdvertisment.ImageId == Guid.Empty ;
        }



        private int MaxSpecialAdvertismentImageFileUploadSize = 1024 * 1024 * 10; //10MB
        private bool OnNewSpecialAdvertismentImageLoading = false;
        private async Task OnNewSpecialAdvertismentImageChanged(InputFileChangeEventArgs e)
        {
            try
            {
                if (e.FileCount is 0 or > 1 || e.File.Size > MaxSpecialAdvertismentImageFileUploadSize)
                {
                    throw new UserFriendlyException(L["UploadFailedMessage"]);
                }
    
                OnNewSpecialAdvertismentImageLoading = true;
                
                var result = await UploadFileAsync(e.File!);
    
                NewSpecialAdvertisment.ImageId = result.Id;
                OnNewSpecialAdvertismentImageLoading = false;            
            }
            catch(Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        private bool HasSelectedSpecialAdvertismentImage = false;
        private bool OnEditSpecialAdvertismentImageLoading = false;
        private async Task OnEditSpecialAdvertismentImageChanged(InputFileChangeEventArgs e)
        {
            try
            {
                if (e.FileCount is 0 or > 1 || e.File.Size > MaxSpecialAdvertismentImageFileUploadSize)
                {
                    throw new UserFriendlyException(L["UploadFailedMessage"]);
                }
    
                OnEditSpecialAdvertismentImageLoading = true;
                
                var result = await UploadFileAsync(e.File!);
    
                EditingSpecialAdvertisment.ImageId = result.Id;
                OnEditSpecialAdvertismentImageLoading = false;            
            }
            catch(Exception ex)
            {
                await HandleErrorAsync(ex);
            }            
        }




        private async Task<AppFileDescriptorDto> UploadFileAsync(IBrowserFile file)
        {
            using (var ms = new MemoryStream())
            {
                await file.OpenReadStream(long.MaxValue).CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                
                return await SpecialAdvertismentsAppService.UploadFileAsync(new RemoteStreamContent(ms, file.Name, file.ContentType));
            }
        }



        private async Task DownloadFileAsync(Guid fileId)
        {
            var token = (await SpecialAdvertismentsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("AhlanFeekumPro") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/special-advertisments/file?DownloadToken={token}&FileId={fileId}", forceLoad: true);
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
        protected virtual async Task OnSitePropertyIdChangedAsync(Guid? sitePropertyId)
        {
            Filter.SitePropertyId = sitePropertyId;
            await SearchAsync();
        }
        

        private async Task GetSitePropertyCollectionLookupAsync(string? newValue = null)
        {
            SitePropertiesCollection = (await SpecialAdvertismentsAppService.GetSitePropertyLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }





        private Task SelectAllItems()
        {
            AllSpecialAdvertismentsSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllSpecialAdvertismentsSelected = false;
            SelectedSpecialAdvertisments.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedSpecialAdvertismentRowsChanged()
        {
            if (SelectedSpecialAdvertisments.Count != PageSize)
            {
                AllSpecialAdvertismentsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedSpecialAdvertismentsAsync()
        {
            var message = AllSpecialAdvertismentsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedSpecialAdvertisments.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllSpecialAdvertismentsSelected)
            {
                await SpecialAdvertismentsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await SpecialAdvertismentsAppService.DeleteByIdsAsync(SelectedSpecialAdvertisments.Select(x => x.SpecialAdvertisment.Id).ToList());
            }

            SelectedSpecialAdvertisments.Clear();
            AllSpecialAdvertismentsSelected = false;

            await GetSpecialAdvertismentsAsync();
        }


    }
}
