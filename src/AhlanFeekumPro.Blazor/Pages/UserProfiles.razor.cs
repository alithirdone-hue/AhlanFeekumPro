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
using AhlanFeekumPro.UserProfiles;
using AhlanFeekumPro.Permissions;
using AhlanFeekumPro.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Volo.Abp;
using Volo.Abp.Content;



namespace AhlanFeekumPro.Blazor.Pages
{
    public partial class UserProfiles
    {
        
        
            
        
            
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<UserProfileWithNavigationPropertiesDto> UserProfileList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateUserProfile { get; set; }
        private bool CanEditUserProfile { get; set; }
        private bool CanDeleteUserProfile { get; set; }
        private UserProfileCreateDto NewUserProfile { get; set; }
        private Validations NewUserProfileValidations { get; set; } = new();
        private UserProfileUpdateDto EditingUserProfile { get; set; }
        private Validations EditingUserProfileValidations { get; set; } = new();
        private Guid EditingUserProfileId { get; set; }
        private Modal CreateUserProfileModal { get; set; } = new();
        private Modal EditUserProfileModal { get; set; } = new();
        private GetUserProfilesInput Filter { get; set; }
        private DataGridEntityActionsColumn<UserProfileWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "userProfile-create-tab";
        protected string SelectedEditTab = "userProfile-edit-tab";
        private UserProfileWithNavigationPropertiesDto? SelectedUserProfile;
        private IReadOnlyList<LookupDto<Guid>> IdentityRolesCollection { get; set; } = new List<LookupDto<Guid>>();
private IReadOnlyList<LookupDto<Guid>> IdentityUsersCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        private List<UserProfileWithNavigationPropertiesDto> SelectedUserProfiles { get; set; } = new();
        private bool AllUserProfilesSelected { get; set; }
        
        public UserProfiles()
        {
            NewUserProfile = new UserProfileCreateDto();
            EditingUserProfile = new UserProfileUpdateDto();
            Filter = new GetUserProfilesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            UserProfileList = new List<UserProfileWithNavigationPropertiesDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetIdentityRoleCollectionLookupAsync();


            await GetIdentityUserCollectionLookupAsync();


            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["UserProfiles"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewUserProfile"], async () =>
            {
                await OpenCreateUserProfileModalAsync();
            }, IconName.Add, requiredPolicyName: AhlanFeekumProPermissions.UserProfiles.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateUserProfile = await AuthorizationService
                .IsGrantedAsync(AhlanFeekumProPermissions.UserProfiles.Create);
            CanEditUserProfile = await AuthorizationService
                            .IsGrantedAsync(AhlanFeekumProPermissions.UserProfiles.Edit);
            CanDeleteUserProfile = await AuthorizationService
                            .IsGrantedAsync(AhlanFeekumProPermissions.UserProfiles.Delete);
                            
                            
        }

        private async Task GetUserProfilesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await UserProfilesAppService.GetListAsync(Filter);
            UserProfileList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetUserProfilesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await UserProfilesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("AhlanFeekumPro") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/user-profiles/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Name={HttpUtility.UrlEncode(Filter.Name)}&Email={HttpUtility.UrlEncode(Filter.Email)}&PhoneNumber={HttpUtility.UrlEncode(Filter.PhoneNumber)}&Latitude={HttpUtility.UrlEncode(Filter.Latitude)}&Longitude={HttpUtility.UrlEncode(Filter.Longitude)}&Address={HttpUtility.UrlEncode(Filter.Address)}&ProfilePhoto={HttpUtility.UrlEncode(Filter.ProfilePhoto)}&IsSuperHost={Filter.IsSuperHost}&IdentityRoleId={Filter.IdentityRoleId}&IdentityUserId={Filter.IdentityUserId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<UserProfileWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetUserProfilesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateUserProfileModalAsync()
        {
            NewUserProfile = new UserProfileCreateDto{
                
                IdentityUserId = IdentityUsersCollection.Select(i=>i.Id).FirstOrDefault(),

            };

            SelectedCreateTab = "userProfile-create-tab";
            
            
            await NewUserProfileValidations.ClearAll();
            await CreateUserProfileModal.Show();
        }

        private async Task CloseCreateUserProfileModalAsync()
        {
            NewUserProfile = new UserProfileCreateDto{
                
                IdentityUserId = IdentityUsersCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateUserProfileModal.Hide();
        }

        private async Task OpenEditUserProfileModalAsync(UserProfileWithNavigationPropertiesDto input)
        {
            SelectedEditTab = "userProfile-edit-tab";
            
            
            var userProfile = await UserProfilesAppService.GetWithNavigationPropertiesAsync(input.UserProfile.Id);
            
            EditingUserProfileId = userProfile.UserProfile.Id;
            EditingUserProfile = ObjectMapper.Map<UserProfileDto, UserProfileUpdateDto>(userProfile.UserProfile);
            
            await EditingUserProfileValidations.ClearAll();
            await EditUserProfileModal.Show();
        }

        private async Task DeleteUserProfileAsync(UserProfileWithNavigationPropertiesDto input)
        {
            await UserProfilesAppService.DeleteAsync(input.UserProfile.Id);
            await GetUserProfilesAsync();
        }

        private async Task CreateUserProfileAsync()
        {
            try
            {
                if (await NewUserProfileValidations.ValidateAll() == false)
                {
                    return;
                }

                await UserProfilesAppService.CreateAsync(NewUserProfile);
                await GetUserProfilesAsync();
                await CloseCreateUserProfileModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditUserProfileModalAsync()
        {
            await EditUserProfileModal.Hide();
        }

        private async Task UpdateUserProfileAsync()
        {
            try
            {
                if (await EditingUserProfileValidations.ValidateAll() == false)
                {
                    return;
                }

                await UserProfilesAppService.UpdateAsync(EditingUserProfileId, EditingUserProfile);
                await GetUserProfilesAsync();
                await EditUserProfileModal.Hide();                
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









        protected virtual async Task OnNameChangedAsync(string? name)
        {
            Filter.Name = name;
            await SearchAsync();
        }
        protected virtual async Task OnEmailChangedAsync(string? email)
        {
            Filter.Email = email;
            await SearchAsync();
        }
        protected virtual async Task OnPhoneNumberChangedAsync(string? phoneNumber)
        {
            Filter.PhoneNumber = phoneNumber;
            await SearchAsync();
        }
        protected virtual async Task OnLatitudeChangedAsync(string? latitude)
        {
            Filter.Latitude = latitude;
            await SearchAsync();
        }
        protected virtual async Task OnLongitudeChangedAsync(string? longitude)
        {
            Filter.Longitude = longitude;
            await SearchAsync();
        }
        protected virtual async Task OnAddressChangedAsync(string? address)
        {
            Filter.Address = address;
            await SearchAsync();
        }
        protected virtual async Task OnProfilePhotoChangedAsync(string? profilePhoto)
        {
            Filter.ProfilePhoto = profilePhoto;
            await SearchAsync();
        }
        protected virtual async Task OnIsSuperHostChangedAsync(bool? isSuperHost)
        {
            Filter.IsSuperHost = isSuperHost;
            await SearchAsync();
        }
        protected virtual async Task OnIdentityRoleIdChangedAsync(Guid? identityRoleId)
        {
            Filter.IdentityRoleId = identityRoleId;
            await SearchAsync();
        }
        protected virtual async Task OnIdentityUserIdChangedAsync(Guid? identityUserId)
        {
            Filter.IdentityUserId = identityUserId;
            await SearchAsync();
        }
        

        private async Task GetIdentityRoleCollectionLookupAsync(string? newValue = null)
        {
            IdentityRolesCollection = (await UserProfilesAppService.GetIdentityRoleLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetIdentityUserCollectionLookupAsync(string? newValue = null)
        {
            IdentityUsersCollection = (await UserProfilesAppService.GetIdentityUserLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }





        private Task SelectAllItems()
        {
            AllUserProfilesSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllUserProfilesSelected = false;
            SelectedUserProfiles.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedUserProfileRowsChanged()
        {
            if (SelectedUserProfiles.Count != PageSize)
            {
                AllUserProfilesSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedUserProfilesAsync()
        {
            var message = AllUserProfilesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedUserProfiles.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllUserProfilesSelected)
            {
                await UserProfilesAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await UserProfilesAppService.DeleteByIdsAsync(SelectedUserProfiles.Select(x => x.UserProfile.Id).ToList());
            }

            SelectedUserProfiles.Clear();
            AllUserProfilesSelected = false;

            await GetUserProfilesAsync();
        }


    }
}
