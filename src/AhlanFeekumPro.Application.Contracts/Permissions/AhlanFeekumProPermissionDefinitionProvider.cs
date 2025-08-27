using AhlanFeekumPro.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace AhlanFeekumPro.Permissions;

public class AhlanFeekumProPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(AhlanFeekumProPermissions.GroupName);

        myGroup.AddPermission(AhlanFeekumProPermissions.Dashboard.Host, L("Permission:Dashboard"), MultiTenancySides.Host);
        myGroup.AddPermission(AhlanFeekumProPermissions.Dashboard.Tenant, L("Permission:Dashboard"), MultiTenancySides.Tenant);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(AhlanFeekumProPermissions.MyPermission1, L("Permission:MyPermission1"));
        var userProfilePermission = myGroup.AddPermission(AhlanFeekumProPermissions.UserProfiles.Default, L("Permission:UserProfiles"));
        userProfilePermission.AddChild(AhlanFeekumProPermissions.UserProfiles.Create, L("Permission:Create"));
        userProfilePermission.AddChild(AhlanFeekumProPermissions.UserProfiles.Edit, L("Permission:Edit"));
        userProfilePermission.AddChild(AhlanFeekumProPermissions.UserProfiles.Delete, L("Permission:Delete"));

        var propertyFeaturePermission = myGroup.AddPermission(AhlanFeekumProPermissions.PropertyFeatures.Default, L("Permission:PropertyFeatures"));
        propertyFeaturePermission.AddChild(AhlanFeekumProPermissions.PropertyFeatures.Create, L("Permission:Create"));
        propertyFeaturePermission.AddChild(AhlanFeekumProPermissions.PropertyFeatures.Edit, L("Permission:Edit"));
        propertyFeaturePermission.AddChild(AhlanFeekumProPermissions.PropertyFeatures.Delete, L("Permission:Delete"));

        var propertyTypePermission = myGroup.AddPermission(AhlanFeekumProPermissions.PropertyTypes.Default, L("Permission:PropertyTypes"));
        propertyTypePermission.AddChild(AhlanFeekumProPermissions.PropertyTypes.Create, L("Permission:Create"));
        propertyTypePermission.AddChild(AhlanFeekumProPermissions.PropertyTypes.Edit, L("Permission:Edit"));
        propertyTypePermission.AddChild(AhlanFeekumProPermissions.PropertyTypes.Delete, L("Permission:Delete"));

        var sitePropertyPermission = myGroup.AddPermission(AhlanFeekumProPermissions.SiteProperties.Default, L("Permission:SiteProperties"));
        sitePropertyPermission.AddChild(AhlanFeekumProPermissions.SiteProperties.Create, L("Permission:Create"));
        sitePropertyPermission.AddChild(AhlanFeekumProPermissions.SiteProperties.Edit, L("Permission:Edit"));
        sitePropertyPermission.AddChild(AhlanFeekumProPermissions.SiteProperties.Delete, L("Permission:Delete"));

        var favoritePropertyPermission = myGroup.AddPermission(AhlanFeekumProPermissions.FavoriteProperties.Default, L("Permission:FavoriteProperties"));
        favoritePropertyPermission.AddChild(AhlanFeekumProPermissions.FavoriteProperties.Create, L("Permission:Create"));
        favoritePropertyPermission.AddChild(AhlanFeekumProPermissions.FavoriteProperties.Edit, L("Permission:Edit"));
        favoritePropertyPermission.AddChild(AhlanFeekumProPermissions.FavoriteProperties.Delete, L("Permission:Delete"));

        var personEvaluationPermission = myGroup.AddPermission(AhlanFeekumProPermissions.PersonEvaluations.Default, L("Permission:PersonEvaluations"));
        personEvaluationPermission.AddChild(AhlanFeekumProPermissions.PersonEvaluations.Create, L("Permission:Create"));
        personEvaluationPermission.AddChild(AhlanFeekumProPermissions.PersonEvaluations.Edit, L("Permission:Edit"));
        personEvaluationPermission.AddChild(AhlanFeekumProPermissions.PersonEvaluations.Delete, L("Permission:Delete"));

        var propertyEvaluationPermission = myGroup.AddPermission(AhlanFeekumProPermissions.PropertyEvaluations.Default, L("Permission:PropertyEvaluations"));
        propertyEvaluationPermission.AddChild(AhlanFeekumProPermissions.PropertyEvaluations.Create, L("Permission:Create"));
        propertyEvaluationPermission.AddChild(AhlanFeekumProPermissions.PropertyEvaluations.Edit, L("Permission:Edit"));
        propertyEvaluationPermission.AddChild(AhlanFeekumProPermissions.PropertyEvaluations.Delete, L("Permission:Delete"));

        var propertyMediaPermission = myGroup.AddPermission(AhlanFeekumProPermissions.PropertyMedias.Default, L("Permission:PropertyMedias"));
        propertyMediaPermission.AddChild(AhlanFeekumProPermissions.PropertyMedias.Create, L("Permission:Create"));
        propertyMediaPermission.AddChild(AhlanFeekumProPermissions.PropertyMedias.Edit, L("Permission:Edit"));
        propertyMediaPermission.AddChild(AhlanFeekumProPermissions.PropertyMedias.Delete, L("Permission:Delete"));

        var verificationCodePermission = myGroup.AddPermission(AhlanFeekumProPermissions.VerificationCodes.Default, L("Permission:VerificationCodes"));
        verificationCodePermission.AddChild(AhlanFeekumProPermissions.VerificationCodes.Create, L("Permission:Create"));
        verificationCodePermission.AddChild(AhlanFeekumProPermissions.VerificationCodes.Edit, L("Permission:Edit"));
        verificationCodePermission.AddChild(AhlanFeekumProPermissions.VerificationCodes.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AhlanFeekumProResource>(name);
    }
}