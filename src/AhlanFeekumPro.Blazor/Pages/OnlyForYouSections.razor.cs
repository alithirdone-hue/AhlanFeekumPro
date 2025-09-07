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
using AhlanFeekumPro.OnlyForYouSections;
using AhlanFeekumPro.Permissions;
using AhlanFeekumPro.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace AhlanFeekumPro.Blazor.Pages
{
    public partial class OnlyForYouSections
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<OnlyForYouSectionDto> OnlyForYouSectionList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateOnlyForYouSection { get; set; }
        private bool CanEditOnlyForYouSection { get; set; }
        private bool CanDeleteOnlyForYouSection { get; set; }
        private OnlyForYouSectionCreateDto NewOnlyForYouSection { get; set; }
        private Validations NewOnlyForYouSectionValidations { get; set; } = new();
        private OnlyForYouSectionUpdateDto EditingOnlyForYouSection { get; set; }
        private Validations EditingOnlyForYouSectionValidations { get; set; } = new();
        private Guid EditingOnlyForYouSectionId { get; set; }
        private Modal CreateOnlyForYouSectionModal { get; set; } = new();
        private Modal EditOnlyForYouSectionModal { get; set; } = new();
        private GetOnlyForYouSectionsInput Filter { get; set; }
        private DataGridEntityActionsColumn<OnlyForYouSectionDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "onlyForYouSection-create-tab";
        protected string SelectedEditTab = "onlyForYouSection-edit-tab";
        private OnlyForYouSectionDto? SelectedOnlyForYouSection;
        
        
        
        
        
        private List<OnlyForYouSectionDto> SelectedOnlyForYouSections { get; set; } = new();
        private bool AllOnlyForYouSectionsSelected { get; set; }
        
        public OnlyForYouSections()
        {
            NewOnlyForYouSection = new OnlyForYouSectionCreateDto();
            EditingOnlyForYouSection = new OnlyForYouSectionUpdateDto();
            Filter = new GetOnlyForYouSectionsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            OnlyForYouSectionList = new List<OnlyForYouSectionDto>();
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["OnlyForYouSections"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewOnlyForYouSection"], async () =>
            {
                await OpenCreateOnlyForYouSectionModalAsync();
            }, IconName.Add, requiredPolicyName: AhlanFeekumProPermissions.OnlyForYouSections.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateOnlyForYouSection = await AuthorizationService
                .IsGrantedAsync(AhlanFeekumProPermissions.OnlyForYouSections.Create);
            CanEditOnlyForYouSection = await AuthorizationService
                            .IsGrantedAsync(AhlanFeekumProPermissions.OnlyForYouSections.Edit);
            CanDeleteOnlyForYouSection = await AuthorizationService
                            .IsGrantedAsync(AhlanFeekumProPermissions.OnlyForYouSections.Delete);
                            
                            
        }

        private async Task GetOnlyForYouSectionsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await OnlyForYouSectionsAppService.GetListAsync(Filter);
            OnlyForYouSectionList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetOnlyForYouSectionsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await OnlyForYouSectionsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("AhlanFeekumPro") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/only-for-you-sections/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&FirstPhoto={HttpUtility.UrlEncode(Filter.FirstPhoto)}&SecondPhoto={HttpUtility.UrlEncode(Filter.SecondPhoto)}&ThirdPhoto={HttpUtility.UrlEncode(Filter.ThirdPhoto)}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<OnlyForYouSectionDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetOnlyForYouSectionsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateOnlyForYouSectionModalAsync()
        {
            NewOnlyForYouSection = new OnlyForYouSectionCreateDto{
                
                
            };

            SelectedCreateTab = "onlyForYouSection-create-tab";
            
            
            await NewOnlyForYouSectionValidations.ClearAll();
            await CreateOnlyForYouSectionModal.Show();
        }

        private async Task CloseCreateOnlyForYouSectionModalAsync()
        {
            NewOnlyForYouSection = new OnlyForYouSectionCreateDto{
                
                
            };
            await CreateOnlyForYouSectionModal.Hide();
        }

        private async Task OpenEditOnlyForYouSectionModalAsync(OnlyForYouSectionDto input)
        {
            SelectedEditTab = "onlyForYouSection-edit-tab";
            
            
            var onlyForYouSection = await OnlyForYouSectionsAppService.GetAsync(input.Id);
            
            EditingOnlyForYouSectionId = onlyForYouSection.Id;
            EditingOnlyForYouSection = ObjectMapper.Map<OnlyForYouSectionDto, OnlyForYouSectionUpdateDto>(onlyForYouSection);
            
            await EditingOnlyForYouSectionValidations.ClearAll();
            await EditOnlyForYouSectionModal.Show();
        }

        private async Task DeleteOnlyForYouSectionAsync(OnlyForYouSectionDto input)
        {
            await OnlyForYouSectionsAppService.DeleteAsync(input.Id);
            await GetOnlyForYouSectionsAsync();
        }

        private async Task CreateOnlyForYouSectionAsync()
        {
            try
            {
                if (await NewOnlyForYouSectionValidations.ValidateAll() == false)
                {
                    return;
                }

                await OnlyForYouSectionsAppService.CreateAsync(NewOnlyForYouSection);
                await GetOnlyForYouSectionsAsync();
                await CloseCreateOnlyForYouSectionModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditOnlyForYouSectionModalAsync()
        {
            await EditOnlyForYouSectionModal.Hide();
        }

        private async Task UpdateOnlyForYouSectionAsync()
        {
            try
            {
                if (await EditingOnlyForYouSectionValidations.ValidateAll() == false)
                {
                    return;
                }

                await OnlyForYouSectionsAppService.UpdateAsync(EditingOnlyForYouSectionId, EditingOnlyForYouSection);
                await GetOnlyForYouSectionsAsync();
                await EditOnlyForYouSectionModal.Hide();                
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









        protected virtual async Task OnFirstPhotoChangedAsync(string? firstPhoto)
        {
            Filter.FirstPhoto = firstPhoto;
            await SearchAsync();
        }
        protected virtual async Task OnSecondPhotoChangedAsync(string? secondPhoto)
        {
            Filter.SecondPhoto = secondPhoto;
            await SearchAsync();
        }
        protected virtual async Task OnThirdPhotoChangedAsync(string? thirdPhoto)
        {
            Filter.ThirdPhoto = thirdPhoto;
            await SearchAsync();
        }
        





        private Task SelectAllItems()
        {
            AllOnlyForYouSectionsSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllOnlyForYouSectionsSelected = false;
            SelectedOnlyForYouSections.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedOnlyForYouSectionRowsChanged()
        {
            if (SelectedOnlyForYouSections.Count != PageSize)
            {
                AllOnlyForYouSectionsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedOnlyForYouSectionsAsync()
        {
            var message = AllOnlyForYouSectionsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedOnlyForYouSections.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllOnlyForYouSectionsSelected)
            {
                await OnlyForYouSectionsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await OnlyForYouSectionsAppService.DeleteByIdsAsync(SelectedOnlyForYouSections.Select(x => x.Id).ToList());
            }

            SelectedOnlyForYouSections.Clear();
            AllOnlyForYouSectionsSelected = false;

            await GetOnlyForYouSectionsAsync();
        }


    }
}
