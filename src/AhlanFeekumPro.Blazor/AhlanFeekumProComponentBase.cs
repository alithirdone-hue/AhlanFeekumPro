using AhlanFeekumPro.Localization;
using Volo.Abp.AspNetCore.Components;

namespace AhlanFeekumPro.Blazor;

public abstract class AhlanFeekumProComponentBase : AbpComponentBase
{
    protected AhlanFeekumProComponentBase()
    {
        LocalizationResource = typeof(AhlanFeekumProResource);
    }
}
