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
using AhlanFeekumPro.SiteProperties;
using AhlanFeekumPro.Permissions;
using AhlanFeekumPro.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace AhlanFeekumPro.Blazor.Pages
{
    public partial class SiteProperties
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<SitePropertyWithNavigationPropertiesDto> SitePropertyList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateSiteProperty { get; set; }
        private bool CanEditSiteProperty { get; set; }
        private bool CanDeleteSiteProperty { get; set; }
        private SitePropertyCreateDto NewSiteProperty { get; set; }
        private Validations NewSitePropertyValidations { get; set; } = new();
        private SitePropertyUpdateDto EditingSiteProperty { get; set; }
        private Validations EditingSitePropertyValidations { get; set; } = new();
        private Guid EditingSitePropertyId { get; set; }
        private Modal CreateSitePropertyModal { get; set; } = new();
        private Modal EditSitePropertyModal { get; set; } = new();
        private GetSitePropertiesInput Filter { get; set; }
        private DataGridEntityActionsColumn<SitePropertyWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "siteProperty-create-tab";
        protected string SelectedEditTab = "siteProperty-edit-tab";
        private SitePropertyWithNavigationPropertiesDto? SelectedSiteProperty;
        private IReadOnlyList<LookupDto<Guid>> PropertyTypesCollection { get; set; } = new List<LookupDto<Guid>>();
private IReadOnlyList<LookupDto<Guid>> PropertyFeatures { get; set; } = new List<LookupDto<Guid>>();
        
        private string SelectedPropertyFeatureId { get; set; }
        
        private string SelectedPropertyFeatureText { get; set; }

        private Blazorise.Components.Autocomplete<LookupDto<Guid>, string> SelectedPropertyFeatureAutoCompleteRef { get; set; } = new();

        private List<LookupDto<Guid>> SelectedPropertyFeatures { get; set; } = new List<LookupDto<Guid>>();
        
        
        
        
        private List<SitePropertyWithNavigationPropertiesDto> SelectedSiteProperties { get; set; } = new();
        private bool AllSitePropertiesSelected { get; set; }
        
        public SiteProperties()
        {
            NewSiteProperty = new SitePropertyCreateDto();
            EditingSiteProperty = new SitePropertyUpdateDto();
            Filter = new GetSitePropertiesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            SitePropertyList = new List<SitePropertyWithNavigationPropertiesDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetPropertyTypeCollectionLookupAsync();


            await GetPropertyFeatureLookupAsync();


            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["SiteProperties"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewSiteProperty"], async () =>
            {
                await OpenCreateSitePropertyModalAsync();
            }, IconName.Add, requiredPolicyName: AhlanFeekumProPermissions.SiteProperties.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateSiteProperty = await AuthorizationService
                .IsGrantedAsync(AhlanFeekumProPermissions.SiteProperties.Create);
            CanEditSiteProperty = await AuthorizationService
                            .IsGrantedAsync(AhlanFeekumProPermissions.SiteProperties.Edit);
            CanDeleteSiteProperty = await AuthorizationService
                            .IsGrantedAsync(AhlanFeekumProPermissions.SiteProperties.Delete);
                            
                            
        }

        private async Task GetSitePropertiesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await SitePropertiesAppService.GetListAsync(Filter);
            SitePropertyList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetSitePropertiesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await SitePropertiesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("AhlanFeekumPro") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/site-properties/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&PropertyTitle={HttpUtility.UrlEncode(Filter.PropertyTitle)}&BedroomsMin={Filter.BedroomsMin}&BedroomsMax={Filter.BedroomsMax}&BathroomsMin={Filter.BathroomsMin}&BathroomsMax={Filter.BathroomsMax}&NumberOfBedMin={Filter.NumberOfBedMin}&NumberOfBedMax={Filter.NumberOfBedMax}&FloorMin={Filter.FloorMin}&FloorMax={Filter.FloorMax}&MaximumNumberOfGuestMin={Filter.MaximumNumberOfGuestMin}&MaximumNumberOfGuestMax={Filter.MaximumNumberOfGuestMax}&LivingroomsMin={Filter.LivingroomsMin}&LivingroomsMax={Filter.LivingroomsMax}&PropertyDescription={HttpUtility.UrlEncode(Filter.PropertyDescription)}&HourseRules={HttpUtility.UrlEncode(Filter.HourseRules)}&ImportantInformation={HttpUtility.UrlEncode(Filter.ImportantInformation)}&Address={HttpUtility.UrlEncode(Filter.Address)}&StreetAndBuildingNumber={HttpUtility.UrlEncode(Filter.StreetAndBuildingNumber)}&LandMark={HttpUtility.UrlEncode(Filter.LandMark)}&PricePerNightMin={Filter.PricePerNightMin}&PricePerNightMax={Filter.PricePerNightMax}&IsActive={Filter.IsActive}&PropertyTypeId={Filter.PropertyTypeId}&PropertyFeatureId={Filter.PropertyFeatureId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<SitePropertyWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetSitePropertiesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateSitePropertyModalAsync()
        {
            SelectedPropertyFeatures = new List<LookupDto<Guid>>();
            SelectedPropertyFeatureId = string.Empty;
            SelectedPropertyFeatureText = string.Empty;

            await SelectedPropertyFeatureAutoCompleteRef.Clear();

            NewSiteProperty = new SitePropertyCreateDto{
                
                PropertyTypeId = PropertyTypesCollection.Select(i=>i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "siteProperty-create-tab";
            
            
            await NewSitePropertyValidations.ClearAll();
            await CreateSitePropertyModal.Show();
        }

        private async Task CloseCreateSitePropertyModalAsync()
        {
            NewSiteProperty = new SitePropertyCreateDto{
                
                PropertyTypeId = PropertyTypesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateSitePropertyModal.Hide();
        }

        private async Task OpenEditSitePropertyModalAsync(SitePropertyWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "siteProperty-edit-tab";
            
            
            var siteProperty = await SitePropertiesAppService.GetWithNavigationPropertiesAsync(input.SiteProperty.Id);
            
            EditingSitePropertyId = siteProperty.SiteProperty.Id;
            EditingSiteProperty = ObjectMapper.Map<SitePropertyDto, SitePropertyUpdateDto>(siteProperty.SiteProperty);
            SelectedPropertyFeatures = siteProperty.PropertyFeatures.Select(a => new LookupDto<Guid>{ Id = a.Id, DisplayName = a.Title}).ToList();

            
            await EditingSitePropertyValidations.ClearAll();
            await EditSitePropertyModal.Show();
        }

        private async Task DeleteSitePropertyAsync(SitePropertyWithNavigationPropertiesDto input)
        {
            await SitePropertiesAppService.DeleteAsync(input.SiteProperty.Id);
            await GetSitePropertiesAsync();
        }

        private async Task CreateSitePropertyAsync()
        {
            try
            {
                if (await NewSitePropertyValidations.ValidateAll() == false)
                {
                    return;
                }
                NewSiteProperty.PropertyFeatureIds = SelectedPropertyFeatures.Select(x => x.Id).ToList();


                await SitePropertiesAppService.CreateAsync(NewSiteProperty);
                await GetSitePropertiesAsync();
                await CloseCreateSitePropertyModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditSitePropertyModalAsync()
        {
            await EditSitePropertyModal.Hide();
        }

        private async Task UpdateSitePropertyAsync()
        {
            try
            {
                if (await EditingSitePropertyValidations.ValidateAll() == false)
                {
                    return;
                }
                EditingSiteProperty.PropertyFeatureIds = SelectedPropertyFeatures.Select(x => x.Id).ToList();


                await SitePropertiesAppService.UpdateAsync(EditingSitePropertyId, EditingSiteProperty);
                await GetSitePropertiesAsync();
                await EditSitePropertyModal.Hide();                
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









        protected virtual async Task OnPropertyTitleChangedAsync(string? propertyTitle)
        {
            Filter.PropertyTitle = propertyTitle;
            await SearchAsync();
        }
        protected virtual async Task OnBedroomsMinChangedAsync(int? bedroomsMin)
        {
            Filter.BedroomsMin = bedroomsMin;
            await SearchAsync();
        }
        protected virtual async Task OnBedroomsMaxChangedAsync(int? bedroomsMax)
        {
            Filter.BedroomsMax = bedroomsMax;
            await SearchAsync();
        }
        protected virtual async Task OnBathroomsMinChangedAsync(int? bathroomsMin)
        {
            Filter.BathroomsMin = bathroomsMin;
            await SearchAsync();
        }
        protected virtual async Task OnBathroomsMaxChangedAsync(int? bathroomsMax)
        {
            Filter.BathroomsMax = bathroomsMax;
            await SearchAsync();
        }
        protected virtual async Task OnNumberOfBedMinChangedAsync(int? numberOfBedMin)
        {
            Filter.NumberOfBedMin = numberOfBedMin;
            await SearchAsync();
        }
        protected virtual async Task OnNumberOfBedMaxChangedAsync(int? numberOfBedMax)
        {
            Filter.NumberOfBedMax = numberOfBedMax;
            await SearchAsync();
        }
        protected virtual async Task OnFloorMinChangedAsync(int? floorMin)
        {
            Filter.FloorMin = floorMin;
            await SearchAsync();
        }
        protected virtual async Task OnFloorMaxChangedAsync(int? floorMax)
        {
            Filter.FloorMax = floorMax;
            await SearchAsync();
        }
        protected virtual async Task OnMaximumNumberOfGuestMinChangedAsync(int? maximumNumberOfGuestMin)
        {
            Filter.MaximumNumberOfGuestMin = maximumNumberOfGuestMin;
            await SearchAsync();
        }
        protected virtual async Task OnMaximumNumberOfGuestMaxChangedAsync(int? maximumNumberOfGuestMax)
        {
            Filter.MaximumNumberOfGuestMax = maximumNumberOfGuestMax;
            await SearchAsync();
        }
        protected virtual async Task OnLivingroomsMinChangedAsync(int? livingroomsMin)
        {
            Filter.LivingroomsMin = livingroomsMin;
            await SearchAsync();
        }
        protected virtual async Task OnLivingroomsMaxChangedAsync(int? livingroomsMax)
        {
            Filter.LivingroomsMax = livingroomsMax;
            await SearchAsync();
        }
        protected virtual async Task OnPropertyDescriptionChangedAsync(string? propertyDescription)
        {
            Filter.PropertyDescription = propertyDescription;
            await SearchAsync();
        }
        protected virtual async Task OnHourseRulesChangedAsync(string? hourseRules)
        {
            Filter.HourseRules = hourseRules;
            await SearchAsync();
        }
        protected virtual async Task OnImportantInformationChangedAsync(string? importantInformation)
        {
            Filter.ImportantInformation = importantInformation;
            await SearchAsync();
        }
        protected virtual async Task OnAddressChangedAsync(string? address)
        {
            Filter.Address = address;
            await SearchAsync();
        }
        protected virtual async Task OnStreetAndBuildingNumberChangedAsync(string? streetAndBuildingNumber)
        {
            Filter.StreetAndBuildingNumber = streetAndBuildingNumber;
            await SearchAsync();
        }
        protected virtual async Task OnLandMarkChangedAsync(string? landMark)
        {
            Filter.LandMark = landMark;
            await SearchAsync();
        }
        protected virtual async Task OnPricePerNightMinChangedAsync(int? pricePerNightMin)
        {
            Filter.PricePerNightMin = pricePerNightMin;
            await SearchAsync();
        }
        protected virtual async Task OnPricePerNightMaxChangedAsync(int? pricePerNightMax)
        {
            Filter.PricePerNightMax = pricePerNightMax;
            await SearchAsync();
        }
        protected virtual async Task OnIsActiveChangedAsync(bool? isActive)
        {
            Filter.IsActive = isActive;
            await SearchAsync();
        }
        protected virtual async Task OnPropertyTypeIdChangedAsync(Guid? propertyTypeId)
        {
            Filter.PropertyTypeId = propertyTypeId;
            await SearchAsync();
        }
        protected virtual async Task OnPropertyFeatureIdChangedAsync(Guid? propertyFeatureId)
        {
            Filter.PropertyFeatureId = propertyFeatureId;
            await SearchAsync();
        }
        

        private async Task GetPropertyTypeCollectionLookupAsync(string? newValue = null)
        {
            PropertyTypesCollection = (await SitePropertiesAppService.GetPropertyTypeLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetPropertyFeatureLookupAsync(string? newValue = null)
        {
            PropertyFeatures = (await SitePropertiesAppService.GetPropertyFeatureLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private void AddPropertyFeature()
        {
            if (SelectedPropertyFeatureId.IsNullOrEmpty())
            {
                return;
            }
            
            if (SelectedPropertyFeatures.Any(p => p.Id.ToString() == SelectedPropertyFeatureId))
            {
                UiMessageService.Warn(L["ItemAlreadyAdded"]);
                return;
            }

            SelectedPropertyFeatures.Add(new LookupDto<Guid>
            {
                Id = Guid.Parse(SelectedPropertyFeatureId),
                DisplayName = SelectedPropertyFeatureText
            });
        }





        private Task SelectAllItems()
        {
            AllSitePropertiesSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllSitePropertiesSelected = false;
            SelectedSiteProperties.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedSitePropertyRowsChanged()
        {
            if (SelectedSiteProperties.Count != PageSize)
            {
                AllSitePropertiesSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedSitePropertiesAsync()
        {
            var message = AllSitePropertiesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedSiteProperties.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllSitePropertiesSelected)
            {
                await SitePropertiesAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await SitePropertiesAppService.DeleteByIdsAsync(SelectedSiteProperties.Select(x => x.SiteProperty.Id).ToList());
            }

            SelectedSiteProperties.Clear();
            AllSitePropertiesSelected = false;

            await GetSitePropertiesAsync();
        }


    }
}
