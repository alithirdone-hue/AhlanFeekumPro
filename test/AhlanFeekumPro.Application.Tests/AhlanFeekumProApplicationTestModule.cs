using Volo.Abp.Modularity;

namespace AhlanFeekumPro;

[DependsOn(
    typeof(AhlanFeekumProApplicationModule),
    typeof(AhlanFeekumProDomainTestModule)
)]
public class AhlanFeekumProApplicationTestModule : AbpModule
{

}
