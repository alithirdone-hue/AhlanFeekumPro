using Microsoft.Extensions.Localization;
using AhlanFeekumPro.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace AhlanFeekumPro;

[Dependency(ReplaceServices = true)]
public class AhlanFeekumProBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<AhlanFeekumProResource> _localizer;

    public AhlanFeekumProBrandingProvider(IStringLocalizer<AhlanFeekumProResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
