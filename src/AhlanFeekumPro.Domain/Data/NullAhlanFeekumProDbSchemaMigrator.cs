using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AhlanFeekumPro.Data;

/* This is used if database provider does't define
 * IAhlanFeekumProDbSchemaMigrator implementation.
 */
public class NullAhlanFeekumProDbSchemaMigrator : IAhlanFeekumProDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
