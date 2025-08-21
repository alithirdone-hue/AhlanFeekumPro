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
}
