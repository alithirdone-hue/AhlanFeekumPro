using Volo.Abp.Modularity;

namespace AhlanFeekumPro;

/* Inherit from this class for your domain layer tests. */
public abstract class AhlanFeekumProDomainTestBase<TStartupModule> : AhlanFeekumProTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
