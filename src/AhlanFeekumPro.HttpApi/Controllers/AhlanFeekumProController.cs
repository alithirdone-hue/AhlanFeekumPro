using AhlanFeekumPro.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace AhlanFeekumPro.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class AhlanFeekumProController : AbpControllerBase
{
    protected AhlanFeekumProController()
    {
        LocalizationResource = typeof(AhlanFeekumProResource);
    }
}
