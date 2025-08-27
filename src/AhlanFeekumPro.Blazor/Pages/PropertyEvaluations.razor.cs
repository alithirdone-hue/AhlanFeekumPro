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
using AhlanFeekumPro.PropertyEvaluations;
using AhlanFeekumPro.Permissions;
using AhlanFeekumPro.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace AhlanFeekumPro.Blazor.Pages
{
    public partial class PropertyEvaluations
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<PropertyEvaluationWithNavigationPropertiesDto> PropertyEvaluationList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreatePropertyEvaluation { get; set; }
        private bool CanEditPropertyEvaluation { get; set; }
        private bool CanDeletePropertyEvaluation { get; set; }
        private PropertyEvaluationCreateDto NewPropertyEvaluation { get; set; }
        private Validations NewPropertyEvaluationValidations { get; set; } = new();
        private PropertyEvaluationUpdateDto EditingPropertyEvaluation { get; set; }
        private Validations EditingPropertyEvaluationValidations { get; set; } = new();
        private Guid EditingPropertyEvaluationId { get; set; }
        private Modal CreatePropertyEvaluationModal { get; set; } = new();
        private Modal EditPropertyEvaluationModal { get; set; } = new();
        private GetPropertyEvaluationsInput Filter { get; set; }
        private DataGridEntityActionsColumn<PropertyEvaluationWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "propertyEvaluation-create-tab";
        protected string SelectedEditTab = "propertyEvaluation-edit-tab";
        private PropertyEvaluationWithNavigationPropertiesDto? SelectedPropertyEvaluation;
        private IReadOnlyList<LookupDto<Guid>> UserProfilesCollection { get; set; } = new List<LookupDto<Guid>>();
private IReadOnlyList<LookupDto<Guid>> SitePropertiesCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        private List<PropertyEvaluationWithNavigationPropertiesDto> SelectedPropertyEvaluations { get; set; } = new();
        private bool AllPropertyEvaluationsSelected { get; set; }
        
        public PropertyEvaluations()
        {
            NewPropertyEvaluation = new PropertyEvaluationCreateDto();
            EditingPropertyEvaluation = new PropertyEvaluationUpdateDto();
            Filter = new GetPropertyEvaluationsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            PropertyEvaluationList = new List<PropertyEvaluationWithNavigationPropertiesDto>();
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["PropertyEvaluations"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewPropertyEvaluation"], async () =>
            {
                await OpenCreatePropertyEvaluationModalAsync();
            }, IconName.Add, requiredPolicyName: AhlanFeekumProPermissions.PropertyEvaluations.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreatePropertyEvaluation = await AuthorizationService
                .IsGrantedAsync(AhlanFeekumProPermissions.PropertyEvaluations.Create);
            CanEditPropertyEvaluation = await AuthorizationService
                            .IsGrantedAsync(AhlanFeekumProPermissions.PropertyEvaluations.Edit);
            CanDeletePropertyEvaluation = await AuthorizationService
                            .IsGrantedAsync(AhlanFeekumProPermissions.PropertyEvaluations.Delete);
                            
                            
        }

        private async Task GetPropertyEvaluationsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await PropertyEvaluationsAppService.GetListAsync(Filter);
            PropertyEvaluationList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetPropertyEvaluationsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await PropertyEvaluationsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("AhlanFeekumPro") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/property-evaluations/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&CleanlinessMin={Filter.CleanlinessMin}&CleanlinessMax={Filter.CleanlinessMax}&PriceAndValueMin={Filter.PriceAndValueMin}&PriceAndValueMax={Filter.PriceAndValueMax}&LocationMin={Filter.LocationMin}&LocationMax={Filter.LocationMax}&AccuracyMin={Filter.AccuracyMin}&AccuracyMax={Filter.AccuracyMax}&AttitudeMin={Filter.AttitudeMin}&AttitudeMax={Filter.AttitudeMax}&RatingComment={HttpUtility.UrlEncode(Filter.RatingComment)}&UserProfileId={Filter.UserProfileId}&SitePropertyId={Filter.SitePropertyId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<PropertyEvaluationWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetPropertyEvaluationsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreatePropertyEvaluationModalAsync()
        {
            NewPropertyEvaluation = new PropertyEvaluationCreateDto{
                
                UserProfileId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),
SitePropertyId = SitePropertiesCollection.Select(i=>i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "propertyEvaluation-create-tab";
            
            
            await NewPropertyEvaluationValidations.ClearAll();
            await CreatePropertyEvaluationModal.Show();
        }

        private async Task CloseCreatePropertyEvaluationModalAsync()
        {
            NewPropertyEvaluation = new PropertyEvaluationCreateDto{
                
                UserProfileId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),
SitePropertyId = SitePropertiesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreatePropertyEvaluationModal.Hide();
        }

        private async Task OpenEditPropertyEvaluationModalAsync(PropertyEvaluationWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "propertyEvaluation-edit-tab";
            
            
            var propertyEvaluation = await PropertyEvaluationsAppService.GetWithNavigationPropertiesAsync(input.PropertyEvaluation.Id);
            
            EditingPropertyEvaluationId = propertyEvaluation.PropertyEvaluation.Id;
            EditingPropertyEvaluation = ObjectMapper.Map<PropertyEvaluationDto, PropertyEvaluationUpdateDto>(propertyEvaluation.PropertyEvaluation);
            
            await EditingPropertyEvaluationValidations.ClearAll();
            await EditPropertyEvaluationModal.Show();
        }

        private async Task DeletePropertyEvaluationAsync(PropertyEvaluationWithNavigationPropertiesDto input)
        {
            await PropertyEvaluationsAppService.DeleteAsync(input.PropertyEvaluation.Id);
            await GetPropertyEvaluationsAsync();
        }

        private async Task CreatePropertyEvaluationAsync()
        {
            try
            {
                if (await NewPropertyEvaluationValidations.ValidateAll() == false)
                {
                    return;
                }

                await PropertyEvaluationsAppService.CreateAsync(NewPropertyEvaluation);
                await GetPropertyEvaluationsAsync();
                await CloseCreatePropertyEvaluationModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditPropertyEvaluationModalAsync()
        {
            await EditPropertyEvaluationModal.Hide();
        }

        private async Task UpdatePropertyEvaluationAsync()
        {
            try
            {
                if (await EditingPropertyEvaluationValidations.ValidateAll() == false)
                {
                    return;
                }

                await PropertyEvaluationsAppService.UpdateAsync(EditingPropertyEvaluationId, EditingPropertyEvaluation);
                await GetPropertyEvaluationsAsync();
                await EditPropertyEvaluationModal.Hide();                
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









        protected virtual async Task OnCleanlinessMinChangedAsync(int? cleanlinessMin)
        {
            Filter.CleanlinessMin = cleanlinessMin;
            await SearchAsync();
        }
        protected virtual async Task OnCleanlinessMaxChangedAsync(int? cleanlinessMax)
        {
            Filter.CleanlinessMax = cleanlinessMax;
            await SearchAsync();
        }
        protected virtual async Task OnPriceAndValueMinChangedAsync(int? priceAndValueMin)
        {
            Filter.PriceAndValueMin = priceAndValueMin;
            await SearchAsync();
        }
        protected virtual async Task OnPriceAndValueMaxChangedAsync(int? priceAndValueMax)
        {
            Filter.PriceAndValueMax = priceAndValueMax;
            await SearchAsync();
        }
        protected virtual async Task OnLocationMinChangedAsync(int? locationMin)
        {
            Filter.LocationMin = locationMin;
            await SearchAsync();
        }
        protected virtual async Task OnLocationMaxChangedAsync(int? locationMax)
        {
            Filter.LocationMax = locationMax;
            await SearchAsync();
        }
        protected virtual async Task OnAccuracyMinChangedAsync(int? accuracyMin)
        {
            Filter.AccuracyMin = accuracyMin;
            await SearchAsync();
        }
        protected virtual async Task OnAccuracyMaxChangedAsync(int? accuracyMax)
        {
            Filter.AccuracyMax = accuracyMax;
            await SearchAsync();
        }
        protected virtual async Task OnAttitudeMinChangedAsync(int? attitudeMin)
        {
            Filter.AttitudeMin = attitudeMin;
            await SearchAsync();
        }
        protected virtual async Task OnAttitudeMaxChangedAsync(int? attitudeMax)
        {
            Filter.AttitudeMax = attitudeMax;
            await SearchAsync();
        }
        protected virtual async Task OnRatingCommentChangedAsync(string? ratingComment)
        {
            Filter.RatingComment = ratingComment;
            await SearchAsync();
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
            UserProfilesCollection = (await PropertyEvaluationsAppService.GetUserProfileLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetSitePropertyCollectionLookupAsync(string? newValue = null)
        {
            SitePropertiesCollection = (await PropertyEvaluationsAppService.GetSitePropertyLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }





        private Task SelectAllItems()
        {
            AllPropertyEvaluationsSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllPropertyEvaluationsSelected = false;
            SelectedPropertyEvaluations.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedPropertyEvaluationRowsChanged()
        {
            if (SelectedPropertyEvaluations.Count != PageSize)
            {
                AllPropertyEvaluationsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedPropertyEvaluationsAsync()
        {
            var message = AllPropertyEvaluationsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedPropertyEvaluations.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllPropertyEvaluationsSelected)
            {
                await PropertyEvaluationsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await PropertyEvaluationsAppService.DeleteByIdsAsync(SelectedPropertyEvaluations.Select(x => x.PropertyEvaluation.Id).ToList());
            }

            SelectedPropertyEvaluations.Clear();
            AllPropertyEvaluationsSelected = false;

            await GetPropertyEvaluationsAsync();
        }


    }
}
