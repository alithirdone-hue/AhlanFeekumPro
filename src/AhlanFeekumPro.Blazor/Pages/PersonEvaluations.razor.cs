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
using AhlanFeekumPro.PersonEvaluations;
using AhlanFeekumPro.Permissions;
using AhlanFeekumPro.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace AhlanFeekumPro.Blazor.Pages
{
    public partial class PersonEvaluations
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<PersonEvaluationWithNavigationPropertiesDto> PersonEvaluationList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreatePersonEvaluation { get; set; }
        private bool CanEditPersonEvaluation { get; set; }
        private bool CanDeletePersonEvaluation { get; set; }
        private PersonEvaluationCreateDto NewPersonEvaluation { get; set; }
        private Validations NewPersonEvaluationValidations { get; set; } = new();
        private PersonEvaluationUpdateDto EditingPersonEvaluation { get; set; }
        private Validations EditingPersonEvaluationValidations { get; set; } = new();
        private Guid EditingPersonEvaluationId { get; set; }
        private Modal CreatePersonEvaluationModal { get; set; } = new();
        private Modal EditPersonEvaluationModal { get; set; } = new();
        private GetPersonEvaluationsInput Filter { get; set; }
        private DataGridEntityActionsColumn<PersonEvaluationWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "personEvaluation-create-tab";
        protected string SelectedEditTab = "personEvaluation-edit-tab";
        private PersonEvaluationWithNavigationPropertiesDto? SelectedPersonEvaluation;
        private IReadOnlyList<LookupDto<Guid>> UserProfilesCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        private List<PersonEvaluationWithNavigationPropertiesDto> SelectedPersonEvaluations { get; set; } = new();
        private bool AllPersonEvaluationsSelected { get; set; }
        
        public PersonEvaluations()
        {
            NewPersonEvaluation = new PersonEvaluationCreateDto();
            EditingPersonEvaluation = new PersonEvaluationUpdateDto();
            Filter = new GetPersonEvaluationsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            PersonEvaluationList = new List<PersonEvaluationWithNavigationPropertiesDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetUserProfileCollectionLookupAsync();


            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["PersonEvaluations"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewPersonEvaluation"], async () =>
            {
                await OpenCreatePersonEvaluationModalAsync();
            }, IconName.Add, requiredPolicyName: AhlanFeekumProPermissions.PersonEvaluations.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreatePersonEvaluation = await AuthorizationService
                .IsGrantedAsync(AhlanFeekumProPermissions.PersonEvaluations.Create);
            CanEditPersonEvaluation = await AuthorizationService
                            .IsGrantedAsync(AhlanFeekumProPermissions.PersonEvaluations.Edit);
            CanDeletePersonEvaluation = await AuthorizationService
                            .IsGrantedAsync(AhlanFeekumProPermissions.PersonEvaluations.Delete);
                            
                            
        }

        private async Task GetPersonEvaluationsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await PersonEvaluationsAppService.GetListAsync(Filter);
            PersonEvaluationList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetPersonEvaluationsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await PersonEvaluationsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("AhlanFeekumPro") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/person-evaluations/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&RateMin={Filter.RateMin}&RateMax={Filter.RateMax}&Comment={HttpUtility.UrlEncode(Filter.Comment)}&EvaluatorId={Filter.EvaluatorId}&EvaluatedPersonId={Filter.EvaluatedPersonId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<PersonEvaluationWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetPersonEvaluationsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreatePersonEvaluationModalAsync()
        {
            NewPersonEvaluation = new PersonEvaluationCreateDto{
                
                EvaluatorId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),
EvaluatedPersonId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "personEvaluation-create-tab";
            
            
            await NewPersonEvaluationValidations.ClearAll();
            await CreatePersonEvaluationModal.Show();
        }

        private async Task CloseCreatePersonEvaluationModalAsync()
        {
            NewPersonEvaluation = new PersonEvaluationCreateDto{
                
                EvaluatorId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),
EvaluatedPersonId = UserProfilesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreatePersonEvaluationModal.Hide();
        }

        private async Task OpenEditPersonEvaluationModalAsync(PersonEvaluationWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "personEvaluation-edit-tab";
            
            
            var personEvaluation = await PersonEvaluationsAppService.GetWithNavigationPropertiesAsync(input.PersonEvaluation.Id);
            
            EditingPersonEvaluationId = personEvaluation.PersonEvaluation.Id;
            EditingPersonEvaluation = ObjectMapper.Map<PersonEvaluationDto, PersonEvaluationUpdateDto>(personEvaluation.PersonEvaluation);
            
            await EditingPersonEvaluationValidations.ClearAll();
            await EditPersonEvaluationModal.Show();
        }

        private async Task DeletePersonEvaluationAsync(PersonEvaluationWithNavigationPropertiesDto input)
        {
            await PersonEvaluationsAppService.DeleteAsync(input.PersonEvaluation.Id);
            await GetPersonEvaluationsAsync();
        }

        private async Task CreatePersonEvaluationAsync()
        {
            try
            {
                if (await NewPersonEvaluationValidations.ValidateAll() == false)
                {
                    return;
                }

                await PersonEvaluationsAppService.CreateAsync(NewPersonEvaluation);
                await GetPersonEvaluationsAsync();
                await CloseCreatePersonEvaluationModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditPersonEvaluationModalAsync()
        {
            await EditPersonEvaluationModal.Hide();
        }

        private async Task UpdatePersonEvaluationAsync()
        {
            try
            {
                if (await EditingPersonEvaluationValidations.ValidateAll() == false)
                {
                    return;
                }

                await PersonEvaluationsAppService.UpdateAsync(EditingPersonEvaluationId, EditingPersonEvaluation);
                await GetPersonEvaluationsAsync();
                await EditPersonEvaluationModal.Hide();                
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









        protected virtual async Task OnRateMinChangedAsync(int? rateMin)
        {
            Filter.RateMin = rateMin;
            await SearchAsync();
        }
        protected virtual async Task OnRateMaxChangedAsync(int? rateMax)
        {
            Filter.RateMax = rateMax;
            await SearchAsync();
        }
        protected virtual async Task OnCommentChangedAsync(string? comment)
        {
            Filter.Comment = comment;
            await SearchAsync();
        }
        protected virtual async Task OnEvaluatorIdChangedAsync(Guid? evaluatorId)
        {
            Filter.EvaluatorId = evaluatorId;
            await SearchAsync();
        }
        protected virtual async Task OnEvaluatedPersonIdChangedAsync(Guid? evaluatedPersonId)
        {
            Filter.EvaluatedPersonId = evaluatedPersonId;
            await SearchAsync();
        }
        

        private async Task GetUserProfileCollectionLookupAsync(string? newValue = null)
        {
            UserProfilesCollection = (await PersonEvaluationsAppService.GetUserProfileLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }





        private Task SelectAllItems()
        {
            AllPersonEvaluationsSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllPersonEvaluationsSelected = false;
            SelectedPersonEvaluations.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedPersonEvaluationRowsChanged()
        {
            if (SelectedPersonEvaluations.Count != PageSize)
            {
                AllPersonEvaluationsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedPersonEvaluationsAsync()
        {
            var message = AllPersonEvaluationsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedPersonEvaluations.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllPersonEvaluationsSelected)
            {
                await PersonEvaluationsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await PersonEvaluationsAppService.DeleteByIdsAsync(SelectedPersonEvaluations.Select(x => x.PersonEvaluation.Id).ToList());
            }

            SelectedPersonEvaluations.Clear();
            AllPersonEvaluationsSelected = false;

            await GetPersonEvaluationsAsync();
        }


    }
}
