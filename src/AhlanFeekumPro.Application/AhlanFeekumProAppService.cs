using AhlanFeekumPro.Localization;
using Volo.Abp.Application.Services;

namespace AhlanFeekumPro;

/* Inherit your application services from this class.
 */
public abstract class AhlanFeekumProAppService : ApplicationService
{
    protected AhlanFeekumProAppService()
    {
        LocalizationResource = typeof(AhlanFeekumProResource);
    }
}
