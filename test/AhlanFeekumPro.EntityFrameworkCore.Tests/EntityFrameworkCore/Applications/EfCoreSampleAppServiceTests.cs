using AhlanFeekumPro.Samples;
using Xunit;

namespace AhlanFeekumPro.EntityFrameworkCore.Applications;

[Collection(AhlanFeekumProTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<AhlanFeekumProEntityFrameworkCoreTestModule>
{

}
