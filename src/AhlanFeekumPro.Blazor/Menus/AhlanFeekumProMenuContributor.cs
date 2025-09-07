using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using AhlanFeekumPro.Localization;
using AhlanFeekumPro.Permissions;
using Volo.Abp.AuditLogging.Blazor.Menus;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Pro.Blazor.Navigation;
using Volo.Abp.LanguageManagement.Blazor.Menus;
using Volo.Abp.OpenIddict.Pro.Blazor.Menus;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.TextTemplateManagement.Blazor.Menus;
using Volo.Abp.UI.Navigation;
using Volo.Saas.Host.Blazor.Navigation;

namespace AhlanFeekumPro.Blazor.Menus;

public class AhlanFeekumProMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<AhlanFeekumProResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                AhlanFeekumProMenus.Home,
                l["Menu:Home"],
                "/",
                icon: "fas fa-home",
                order: 1
            )
        );

        //HostDashboard
        context.Menu.AddItem(
            new ApplicationMenuItem(
                AhlanFeekumProMenus.HostDashboard,
                l["Menu:Dashboard"],
                "~/HostDashboard",
                icon: "fa fa-line-chart",
                order: 2
            ).RequirePermissions(AhlanFeekumProPermissions.Dashboard.Host)
        );

        //TenantDashboard
        context.Menu.AddItem(
            new ApplicationMenuItem(
                AhlanFeekumProMenus.TenantDashboard,
                l["Menu:Dashboard"],
                "~/Dashboard",
                icon: "fa fa-line-chart",
                order: 2
            ).RequirePermissions(AhlanFeekumProPermissions.Dashboard.Tenant)
        );

        /* Example nested menu definition:

        context.Menu.AddItem(
            new ApplicationMenuItem("Menu0", "Menu Level 0")
            .AddItem(new ApplicationMenuItem("Menu0.1", "Menu Level 0.1", url: "/test01"))
            .AddItem(
                new ApplicationMenuItem("Menu0.2", "Menu Level 0.2")
                    .AddItem(new ApplicationMenuItem("Menu0.2.1", "Menu Level 0.2.1", url: "/test021"))
                    .AddItem(new ApplicationMenuItem("Menu0.2.2", "Menu Level 0.2.2")
                        .AddItem(new ApplicationMenuItem("Menu0.2.2.1", "Menu Level 0.2.2.1", "/test0221"))
                        .AddItem(new ApplicationMenuItem("Menu0.2.2.2", "Menu Level 0.2.2.2", "/test0222"))
                    )
                    .AddItem(new ApplicationMenuItem("Menu0.2.3", "Menu Level 0.2.3", url: "/test023"))
                    .AddItem(new ApplicationMenuItem("Menu0.2.4", "Menu Level 0.2.4", url: "/test024")
                        .AddItem(new ApplicationMenuItem("Menu0.2.4.1", "Menu Level 0.2.4.1", "/test0241"))
                )
                .AddItem(new ApplicationMenuItem("Menu0.2.5", "Menu Level 0.2.5", url: "/test025"))
            )
            .AddItem(new ApplicationMenuItem("Menu0.2", "Menu Level 0.2", url: "/test02"))
        );

        */

        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 4;

        //Administration->Identity
        administration.SetSubItemOrder(IdentityProMenus.GroupName, 1);

        //Administration->Saas
        administration.SetSubItemOrder(SaasHostMenus.GroupName, 2);

        //Administration->OpenIddict
        administration.SetSubItemOrder(OpenIddictProMenus.GroupName, 3);

        //Administration->Language Management
        administration.SetSubItemOrder(LanguageManagementMenus.GroupName, 5);

        //Administration->Text Template Management
        administration.SetSubItemOrder(TextTemplateManagementMenus.GroupName, 6);

        //Administration->Audit Logs
        administration.SetSubItemOrder(AbpAuditLoggingMenus.GroupName, 7);

        //Administration->Settings
        administration.SetSubItemOrder(SettingManagementMenus.GroupName, 8);

        context.Menu.AddItem(
            new ApplicationMenuItem(
                AhlanFeekumProMenus.UserProfiles,
                l["Menu:UserProfiles"],
                url: "/user-profiles",
icon: "fa fa-file-alt",
                requiredPermissionName: AhlanFeekumProPermissions.UserProfiles.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                AhlanFeekumProMenus.PropertyFeatures,
                l["Menu:PropertyFeatures"],
                url: "/property-features",
                icon: "fa fa-file-alt",
                requiredPermissionName: AhlanFeekumProPermissions.PropertyFeatures.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                AhlanFeekumProMenus.PropertyTypes,
                l["Menu:PropertyTypes"],
                url: "/property-types",
                icon: "fa fa-file-alt",
                requiredPermissionName: AhlanFeekumProPermissions.PropertyTypes.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                AhlanFeekumProMenus.SiteProperties,
                l["Menu:SiteProperties"],
                url: "/site-properties",
icon: "fa fa-file-alt",
                requiredPermissionName: AhlanFeekumProPermissions.SiteProperties.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                AhlanFeekumProMenus.FavoriteProperties,
                l["Menu:FavoriteProperties"],
                url: "/favorite-properties",
                icon: "fa fa-file-alt",
                requiredPermissionName: AhlanFeekumProPermissions.FavoriteProperties.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                AhlanFeekumProMenus.PersonEvaluations,
                l["Menu:PersonEvaluations"],
                url: "/person-evaluations",
                icon: "fa fa-file-alt",
                requiredPermissionName: AhlanFeekumProPermissions.PersonEvaluations.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                AhlanFeekumProMenus.PropertyEvaluations,
                l["Menu:PropertyEvaluations"],
                url: "/property-evaluations",
                icon: "fa fa-file-alt",
                requiredPermissionName: AhlanFeekumProPermissions.PropertyEvaluations.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                AhlanFeekumProMenus.PropertyMedias,
                l["Menu:PropertyMedias"],
                url: "/property-medias",
                icon: "fa fa-file-alt",
                requiredPermissionName: AhlanFeekumProPermissions.PropertyMedias.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                AhlanFeekumProMenus.VerificationCodes,
                l["Menu:VerificationCodes"],
                url: "/verification-codes",
                icon: "fa fa-file-alt",
                requiredPermissionName: AhlanFeekumProPermissions.VerificationCodes.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                AhlanFeekumProMenus.Governorates,
                l["Menu:Governorates"],
                url: "/governorates",
                icon: "fa fa-file-alt",
                requiredPermissionName: AhlanFeekumProPermissions.Governorates.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                AhlanFeekumProMenus.SpecialAdvertisments,
                l["Menu:SpecialAdvertisments"],
                url: "/special-advertisments",
icon: "fa fa-file-alt",
                requiredPermissionName: AhlanFeekumProPermissions.SpecialAdvertisments.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                AhlanFeekumProMenus.OnlyForYouSections,
                l["Menu:OnlyForYouSections"],
                url: "/only-for-you-sections",
                icon: "fa fa-file-alt",
                requiredPermissionName: AhlanFeekumProPermissions.OnlyForYouSections.Default)
        );
        return Task.CompletedTask;
    }
}