using AhlanFeekumPro.VerificationCodes;
using AhlanFeekumPro.PropertyMedias;
using AhlanFeekumPro.PropertyEvaluations;
using AhlanFeekumPro.PersonEvaluations;
using AhlanFeekumPro.FavoriteProperties;
using AhlanFeekumPro.SiteProperties;
using AhlanFeekumPro.PropertyTypes;
using AhlanFeekumPro.PropertyFeatures;
using AhlanFeekumPro.UserProfiles;
using System;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Uow;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.LanguageManagement.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TextTemplateManagement.EntityFrameworkCore;
using Volo.Saas.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Gdpr;
using Volo.Abp.OpenIddict.EntityFrameworkCore;

namespace AhlanFeekumPro.EntityFrameworkCore;

[DependsOn(
    typeof(AhlanFeekumProDomainModule),
    typeof(AbpIdentityProEntityFrameworkCoreModule),
    typeof(AbpOpenIddictProEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqlServerModule),
    typeof(AbpBackgroundJobsEntityFrameworkCoreModule),
    typeof(AbpAuditLoggingEntityFrameworkCoreModule),
    typeof(AbpFeatureManagementEntityFrameworkCoreModule),
    typeof(LanguageManagementEntityFrameworkCoreModule),
    typeof(SaasEntityFrameworkCoreModule),
    typeof(TextTemplateManagementEntityFrameworkCoreModule),
    typeof(AbpGdprEntityFrameworkCoreModule),
    typeof(BlobStoringDatabaseEntityFrameworkCoreModule)
    )]
public class AhlanFeekumProEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        AhlanFeekumProEfCoreEntityExtensionMappings.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<AhlanFeekumProDbContext>(options =>
        {
            /* Remove "includeAllEntities: true" to create
             * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);
            options.AddRepository<UserProfile, UserProfiles.EfCoreUserProfileRepository>();

            options.AddRepository<PropertyFeature, PropertyFeatures.EfCorePropertyFeatureRepository>();

            options.AddRepository<PropertyType, PropertyTypes.EfCorePropertyTypeRepository>();

            options.AddRepository<SiteProperty, SiteProperties.EfCoreSitePropertyRepository>();

            options.AddRepository<FavoriteProperty, FavoriteProperties.EfCoreFavoritePropertyRepository>();

            options.AddRepository<PersonEvaluation, PersonEvaluations.EfCorePersonEvaluationRepository>();

            options.AddRepository<PropertyEvaluation, PropertyEvaluations.EfCorePropertyEvaluationRepository>();

            options.AddRepository<PropertyMedia, PropertyMedias.EfCorePropertyMediaRepository>();

            options.AddRepository<VerificationCode, VerificationCodes.EfCoreVerificationCodeRepository>();

        });

        Configure<AbpDbContextOptions>(options =>
        {
            /* The main point to change your DBMS.
             * See also AhlanFeekumProDbContextFactory for EF Core tooling. */
            options.UseSqlServer();
        });

    }
}