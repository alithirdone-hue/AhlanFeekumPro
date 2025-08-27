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
using AhlanFeekumPro.FavoriteProperties;
using AhlanFeekumPro.Permissions;
using AhlanFeekumPro.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace AhlanFeekumPro.Blazor.Pages
{
    public partial class FavoriteProperties
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<FavoritePropertyWithNavigationPropertiesDto> FavoritePropertyList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateFavoriteProperty { get; set; }
        private bool CanEditFavoriteProperty { get; set; }
        private bool CanDeleteFavoriteProperty { get; set; }
        private FavoritePropertyCreateDto NewFavoriteProperty { get; set; }
        private Validations NewFavoritePropertyValidations { get; set; } = new();
        private FavoritePropertyUpdateDto EditingFavoriteProperty { get; set; }
        private Validations EditingFavoritePropertyValidations { get; set; } = new();
        private Guid EditingFavoritePropertyId { get; set; }
        private Modal CreateFavoritePropertyModal { get; set; } = new();
        private Modal EditFavoritePropertyModal { get; set; } = new();
        private GetFavoritePropertiesInput Filter { get; set; }
        private DataGridEntityActionsColumn<FavoritePropertyWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "favoriteProperty-create-tab";
        protected string SelectedEditTab = "favoriteProperty-edit-tab";
        private FavoritePropertyWithNavigationPropertiesDto? SelectedFavoriteProperty;
        private IReadOnlyList<LookupDto<Guid>> UserProfilesCollection { get; set; } = new List<LookupDto<Guid>>();
private IReadOnlyList<LookupDto<Guid>> SitePropertiesCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        private List<FavoritePropertyWithNavigationPropertiesDto> SelectedFavoriteProperties { get; set; } = new();
        private bool AllFavoritePropertiesSelected { get; set; }
        
        public FavoriteProperties()
        {
            NewFavoriteProperty = new FavoritePropertyCreateDto();
            EditingFavoriteProperty = new FavoritePropertyUpdateDto();
            Filter = new GetFavoritePropertiesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            FavoritePropertyList = new List<FavoritePropertyWithNavigationPropertiesDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetUserProfileCollectionLookupAsync();


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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["FavoriteProperties"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewFavoriteProperty"], async () =>
            {
                await OpenCreateFavoritePropertyModalAsync();
            }, IconName.Add, requiredPolicyName: AhlanFeekumProPermissions.FavoriteProperties.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateFavoriteProperty = await AuthorizationService
                .IsGrantedAsync(AhlanFeekumProPermissions.FavoriteProperties.Create);
            CanEditFavoriteProperty = await AuthorizationService
                            .IsGrantedAsync(AhlanFeekumProPermissions.FavoriteProperties.Edit);
            CanDeleteFavoriteProperty = await AuthorizationService
                            .IsGrantedAsync(AhlanFeekumProPermissions.FavoriteProperties.Delete);
                            
                            
        }

        private async Task GetFavoritePropertiesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await FavoritePropertiesAppService.GetListAsync(Filter);
            FavoritePropertyList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetFavoritePropertiesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await FavoritePropertiesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("AhlanFeekumPro") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/favorite-properties/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&UserProfileId={Filter.UserProfileId}&SitePropertyId={Filter.SitePropertyId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<FavoritePropertyWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetFavoritePropertiesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateFavoritePropertyModalAsync()
        {
            NewFavoriteProperty = new FavoritePropertyCreateDto{
                
                UserProfileId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),
SitePropertyId = SitePropertiesCollection.Select(i=>i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "favoriteProperty-create-tab";
            
            
            await NewFavoritePropertyValidations.ClearAll();
            await CreateFavoritePropertyModal.Show();
        }

        private async Task CloseCreateFavoritePropertyModalAsync()
        {
            NewFavoriteProperty = new FavoritePropertyCreateDto{
                
                UserProfileId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),
SitePropertyId = SitePropertiesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateFavoritePropertyModal.Hide();
        }

        private async Task OpenEditFavoritePropertyModalAsync(FavoritePropertyWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "favoriteProperty-edit-tab";
            
            
            var favoriteProperty = await FavoritePropertiesAppService.GetWithNavigationPropertiesAsync(input.FavoriteProperty.Id);
            
            EditingFavoritePropertyId = favoriteProperty.FavoriteProperty.Id;
            EditingFavoriteProperty = ObjectMapper.Map<FavoritePropertyDto, FavoritePropertyUpdateDto>(favoriteProperty.FavoriteProperty);
            
            await EditingFavoritePropertyValidations.ClearAll();
            await EditFavoritePropertyModal.Show();
        }

        private async Task DeleteFavoritePropertyAsync(FavoritePropertyWithNavigationPropertiesDto input)
        {
            await FavoritePropertiesAppService.DeleteAsync(input.FavoriteProperty.Id);
            await GetFavoritePropertiesAsync();
        }

        private async Task CreateFavoritePropertyAsync()
        {
            try
            {
                if (await NewFavoritePropertyValidations.ValidateAll() == false)
                {
                    return;
                }

                await FavoritePropertiesAppService.CreateAsync(NewFavoriteProperty);
                await GetFavoritePropertiesAsync();
                await CloseCreateFavoritePropertyModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditFavoritePropertyModalAsync()
        {
            await EditFavoritePropertyModal.Hide();
        }

        private async Task UpdateFavoritePropertyAsync()
        {
            try
            {
                if (await EditingFavoritePropertyValidations.ValidateAll() == false)
                {
                    return;
                }

                await FavoritePropertiesAppService.UpdateAsync(EditingFavoritePropertyId, EditingFavoriteProperty);
                await GetFavoritePropertiesAsync();
                await EditFavoritePropertyModal.Hide();                
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









        protected virtual async Task OnUserProfileIdChangedAsync(Guid? userProfileId)
        {
            Filter.UserProfileId = userProfileId;
            await SearchAsync();
        }
        protected virtual async Task OnSitePropertyIdChangedAsync(Guid? sitePropertyId)
        {
            Filter.SitePropertyId = sitePropertyId;
            await SearchAsync();
        }
        

        private async Task GetUserProfileCollectionLookupAsync(string? newValue = null)
        {
            UserProfilesCollection = (await FavoritePropertiesAppService.GetUserProfileLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetSitePropertyCollectionLookupAsync(string? newValue = null)
        {
            SitePropertiesCollection = (await FavoritePropertiesAppService.GetSitePropertyLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }





        private Task SelectAllItems()
        {
            AllFavoritePropertiesSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllFavoritePropertiesSelected = false;
            SelectedFavoriteProperties.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedFavoritePropertyRowsChanged()
        {
            if (SelectedFavoriteProperties.Count != PageSize)
            {
                AllFavoritePropertiesSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedFavoritePropertiesAsync()
        {
            var message = AllFavoritePropertiesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedFavoriteProperties.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllFavoritePropertiesSelected)
            {
                await FavoritePropertiesAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await FavoritePropertiesAppService.DeleteByIdsAsync(SelectedFavoriteProperties.Select(x => x.FavoriteProperty.Id).ToList());
            }

            SelectedFavoriteProperties.Clear();
            AllFavoritePropertiesSelected = false;

            await GetFavoritePropertiesAsync();
        }


    }
}
