using AhlanFeekumPro.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace AhlanFeekumPro.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AhlanFeekumProEntityFrameworkCoreModule),
    typeof(AhlanFeekumProApplicationContractsModule)
)]
public class AhlanFeekumProDbMigratorModule : AbpModule
{
}
