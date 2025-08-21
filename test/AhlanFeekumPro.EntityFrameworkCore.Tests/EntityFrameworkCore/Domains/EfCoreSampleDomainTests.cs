using AhlanFeekumPro.Samples;
using Xunit;

namespace AhlanFeekumPro.EntityFrameworkCore.Domains;

[Collection(AhlanFeekumProTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<AhlanFeekumProEntityFrameworkCoreTestModule>
{

}
