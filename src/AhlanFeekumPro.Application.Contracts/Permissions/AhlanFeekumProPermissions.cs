namespace AhlanFeekumPro.Permissions;

public static class AhlanFeekumProPermissions
{
    public const string GroupName = "AhlanFeekumPro";

    public static class Dashboard
    {
        public const string DashboardGroup = GroupName + ".Dashboard";
        public const string Host = DashboardGroup + ".Host";
        public const string Tenant = DashboardGroup + ".Tenant";
    }

    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";
    public static class UserProfiles
    {
        public const string Default = GroupName + ".UserProfiles";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class PropertyFeatures
    {
        public const string Default = GroupName + ".PropertyFeatures";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class PropertyTypes
    {
        public const string Default = GroupName + ".PropertyTypes";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class SiteProperties
    {
        public const string Default = GroupName + ".SiteProperties";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class FavoriteProperties
    {
        public const string Default = GroupName + ".FavoriteProperties";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class PersonEvaluations
    {
        public const string Default = GroupName + ".PersonEvaluations";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class PropertyEvaluations
    {
        public const string Default = GroupName + ".PropertyEvaluations";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class PropertyMedias
    {
        public const string Default = GroupName + ".PropertyMedias";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class VerificationCodes
    {
        public const string Default = GroupName + ".VerificationCodes";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class Governorates
    {
        public const string Default = GroupName + ".Governorates";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class SpecialAdvertisments
    {
        public const string Default = GroupName + ".SpecialAdvertisments";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class OnlyForYouSections
    {
        public const string Default = GroupName + ".OnlyForYouSections";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class PropertyCalendars
    {
        public const string Default = GroupName + ".PropertyCalendars";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
}