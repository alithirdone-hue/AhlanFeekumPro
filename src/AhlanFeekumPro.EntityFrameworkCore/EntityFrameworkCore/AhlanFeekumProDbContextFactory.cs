using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AhlanFeekumPro.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class AhlanFeekumProDbContextFactory : IDesignTimeDbContextFactory<AhlanFeekumProDbContext>
{
    public AhlanFeekumProDbContext CreateDbContext(string[] args)
    {
        AhlanFeekumProEfCoreEntityExtensionMappings.Configure();

        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<AhlanFeekumProDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));

        return new AhlanFeekumProDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../AhlanFeekumPro.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
