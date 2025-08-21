using Volo.Abp.Modularity;

namespace AhlanFeekumPro;

[DependsOn(
    typeof(AhlanFeekumProDomainModule),
    typeof(AhlanFeekumProTestBaseModule)
)]
public class AhlanFeekumProDomainTestModule : AbpModule
{

}
