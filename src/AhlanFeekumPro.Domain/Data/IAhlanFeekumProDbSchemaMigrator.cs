using System.Threading.Tasks;

namespace AhlanFeekumPro.Data;

public interface IAhlanFeekumProDbSchemaMigrator
{
    Task MigrateAsync();
}
