using AhlanFeekumPro.PropertyCalendars;
using AhlanFeekumPro.OnlyForYouSections;
using AhlanFeekumPro.SpecialAdvertisments;
using AhlanFeekumPro.Governorates;
using AhlanFeekumPro.VerificationCodes;
using AhlanFeekumPro.PropertyMedias;
using AhlanFeekumPro.PropertyEvaluations;
using AhlanFeekumPro.PersonEvaluations;
using AhlanFeekumPro.FavoriteProperties;
using AhlanFeekumPro.SiteProperties;
using AhlanFeekumPro.PropertyTypes;
using AhlanFeekumPro.PropertyFeatures;
using AhlanFeekumPro.UserProfiles;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.LanguageManagement.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TextTemplateManagement.EntityFrameworkCore;
using Volo.Saas.EntityFrameworkCore;
using Volo.Saas.Editions;
using Volo.Saas.Tenants;
using Volo.Abp.Gdpr;
using Volo.Abp.OpenIddict.EntityFrameworkCore;

namespace AhlanFeekumPro.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityProDbContext))]
[ReplaceDbContext(typeof(ISaasDbContext))]
[ConnectionStringName("Default")]
public class AhlanFeekumProDbContext :
    AbpDbContext<AhlanFeekumProDbContext>,
    IIdentityProDbContext,
    ISaasDbContext
{
    public DbSet<PropertyCalendar> PropertyCalendars { get; set; } = null!;
    public DbSet<OnlyForYouSection> OnlyForYouSections { get; set; } = null!;
    public DbSet<SpecialAdvertisment> SpecialAdvertisments { get; set; } = null!;
    public DbSet<Governorate> Governorates { get; set; } = null!;
    public DbSet<VerificationCode> VerificationCodes { get; set; } = null!;
    public DbSet<PropertyMedia> PropertyMedias { get; set; } = null!;
    public DbSet<PropertyEvaluation> PropertyEvaluations { get; set; } = null!;
    public DbSet<PersonEvaluation> PersonEvaluations { get; set; } = null!;
    public DbSet<FavoriteProperty> FavoriteProperties { get; set; } = null!;
    public DbSet<SiteProperty> SiteProperties { get; set; } = null!;
    public DbSet<PropertyType> PropertyTypes { get; set; } = null!;
    public DbSet<PropertyFeature> PropertyFeatures { get; set; } = null!;
    public DbSet<UserProfile> UserProfiles { get; set; } = null!;
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext and ISaasDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext and ISaasDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    // SaaS
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Edition> Editions { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public AhlanFeekumProDbContext(DbContextOptions<AhlanFeekumProDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentityPro();
        builder.ConfigureOpenIddictPro();
        builder.ConfigureFeatureManagement();
        builder.ConfigureLanguageManagement();
        builder.ConfigureSaas();
        builder.ConfigureTextTemplateManagement();
        builder.ConfigureBlobStoring();
        builder.ConfigureGdpr();

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(AhlanFeekumProConsts.DbTablePrefix + "YourEntities", AhlanFeekumProConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<PropertyFeature>(b =>
            {
                b.ToTable(AhlanFeekumProConsts.DbTablePrefix + "PropertyFeatures", AhlanFeekumProConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Title).HasColumnName(nameof(PropertyFeature.Title)).IsRequired();
                b.Property(x => x.Icon).HasColumnName(nameof(PropertyFeature.Icon)).IsRequired();
                b.Property(x => x.Order).HasColumnName(nameof(PropertyFeature.Order));
                b.Property(x => x.IsActive).HasColumnName(nameof(PropertyFeature.IsActive));
            });

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<UserProfile>(b =>
            {
                b.ToTable(AhlanFeekumProConsts.DbTablePrefix + "UserProfiles", AhlanFeekumProConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Name).HasColumnName(nameof(UserProfile.Name)).IsRequired();
                b.Property(x => x.Email).HasColumnName(nameof(UserProfile.Email));
                b.Property(x => x.PhoneNumber).HasColumnName(nameof(UserProfile.PhoneNumber));
                b.Property(x => x.Latitude).HasColumnName(nameof(UserProfile.Latitude));
                b.Property(x => x.Longitude).HasColumnName(nameof(UserProfile.Longitude));
                b.Property(x => x.Address).HasColumnName(nameof(UserProfile.Address));
                b.Property(x => x.ProfilePhoto).HasColumnName(nameof(UserProfile.ProfilePhoto));
                b.Property(x => x.IsSuperHost).HasColumnName(nameof(UserProfile.IsSuperHost));
                b.HasOne<IdentityRole>().WithMany().HasForeignKey(x => x.IdentityRoleId).OnDelete(DeleteBehavior.SetNull);
                b.HasOne<IdentityUser>().WithMany().IsRequired().HasForeignKey(x => x.IdentityUserId).OnDelete(DeleteBehavior.NoAction);
            });

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<PropertyType>(b =>
            {
                b.ToTable(AhlanFeekumProConsts.DbTablePrefix + "PropertyTypes", AhlanFeekumProConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Title).HasColumnName(nameof(PropertyType.Title)).IsRequired();
                b.Property(x => x.Order).HasColumnName(nameof(PropertyType.Order));
                b.Property(x => x.IsActive).HasColumnName(nameof(PropertyType.IsActive));
            });

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<FavoriteProperty>(b =>
            {
                b.ToTable(AhlanFeekumProConsts.DbTablePrefix + "FavoriteProperties", AhlanFeekumProConsts.DbSchema);
                b.ConfigureByConvention();
                b.HasOne<UserProfile>().WithMany().IsRequired().HasForeignKey(x => x.UserProfileId).OnDelete(DeleteBehavior.NoAction);
                b.HasOne<SiteProperty>().WithMany().IsRequired().HasForeignKey(x => x.SitePropertyId).OnDelete(DeleteBehavior.NoAction);
            });

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<PersonEvaluation>(b =>
            {
                b.ToTable(AhlanFeekumProConsts.DbTablePrefix + "PersonEvaluations", AhlanFeekumProConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Rate).HasColumnName(nameof(PersonEvaluation.Rate)).IsRequired().HasMaxLength(PersonEvaluationConsts.RateMaxLength);
                b.Property(x => x.Comment).HasColumnName(nameof(PersonEvaluation.Comment));
                b.HasOne<UserProfile>().WithMany().IsRequired().HasForeignKey(x => x.EvaluatorId).OnDelete(DeleteBehavior.NoAction);
                b.HasOne<UserProfile>().WithMany().IsRequired().HasForeignKey(x => x.EvaluatedPersonId).OnDelete(DeleteBehavior.NoAction);
            });

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<PropertyEvaluation>(b =>
            {
                b.ToTable(AhlanFeekumProConsts.DbTablePrefix + "PropertyEvaluations", AhlanFeekumProConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Cleanliness).HasColumnName(nameof(PropertyEvaluation.Cleanliness)).IsRequired().HasMaxLength(PropertyEvaluationConsts.CleanlinessMaxLength);
                b.Property(x => x.PriceAndValue).HasColumnName(nameof(PropertyEvaluation.PriceAndValue)).IsRequired().HasMaxLength(PropertyEvaluationConsts.PriceAndValueMaxLength);
                b.Property(x => x.Location).HasColumnName(nameof(PropertyEvaluation.Location)).IsRequired().HasMaxLength(PropertyEvaluationConsts.LocationMaxLength);
                b.Property(x => x.Accuracy).HasColumnName(nameof(PropertyEvaluation.Accuracy)).IsRequired().HasMaxLength(PropertyEvaluationConsts.AccuracyMaxLength);
                b.Property(x => x.Attitude).HasColumnName(nameof(PropertyEvaluation.Attitude)).IsRequired().HasMaxLength(PropertyEvaluationConsts.AttitudeMaxLength);
                b.Property(x => x.RatingComment).HasColumnName(nameof(PropertyEvaluation.RatingComment));
                b.HasOne<UserProfile>().WithMany().IsRequired().HasForeignKey(x => x.UserProfileId).OnDelete(DeleteBehavior.NoAction);
                b.HasOne<SiteProperty>().WithMany().IsRequired().HasForeignKey(x => x.SitePropertyId).OnDelete(DeleteBehavior.NoAction);
            });

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<PropertyMedia>(b =>
            {
                b.ToTable(AhlanFeekumProConsts.DbTablePrefix + "PropertyMedias", AhlanFeekumProConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Image).HasColumnName(nameof(PropertyMedia.Image)).IsRequired();
                b.Property(x => x.Order).HasColumnName(nameof(PropertyMedia.Order));
                b.Property(x => x.isActive).HasColumnName(nameof(PropertyMedia.isActive));
                b.HasOne<SiteProperty>().WithMany().IsRequired().HasForeignKey(x => x.SitePropertyId).OnDelete(DeleteBehavior.NoAction);
            });

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<VerificationCode>(b =>
            {
                b.ToTable(AhlanFeekumProConsts.DbTablePrefix + "VerificationCodes", AhlanFeekumProConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.PhoneOrEmail).HasColumnName(nameof(VerificationCode.PhoneOrEmail)).IsRequired();
                b.Property(x => x.SecurityCode).HasColumnName(nameof(VerificationCode.SecurityCode));
                b.Property(x => x.IsExpired).HasColumnName(nameof(VerificationCode.IsExpired));
            });

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<Governorate>(b =>
            {
                b.ToTable(AhlanFeekumProConsts.DbTablePrefix + "Governorates", AhlanFeekumProConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Title).HasColumnName(nameof(Governorate.Title)).IsRequired();
                b.Property(x => x.Order).HasColumnName(nameof(Governorate.Order));
                b.Property(x => x.IsActive).HasColumnName(nameof(Governorate.IsActive));
            });

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<SiteProperty>(b =>
            {
                b.ToTable(AhlanFeekumProConsts.DbTablePrefix + "SiteProperties", AhlanFeekumProConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.PropertyTitle).HasColumnName(nameof(SiteProperty.PropertyTitle)).IsRequired();
                b.Property(x => x.HotelName).HasColumnName(nameof(SiteProperty.HotelName));
                b.Property(x => x.Bedrooms).HasColumnName(nameof(SiteProperty.Bedrooms));
                b.Property(x => x.Bathrooms).HasColumnName(nameof(SiteProperty.Bathrooms));
                b.Property(x => x.NumberOfBed).HasColumnName(nameof(SiteProperty.NumberOfBed));
                b.Property(x => x.Floor).HasColumnName(nameof(SiteProperty.Floor));
                b.Property(x => x.MaximumNumberOfGuest).HasColumnName(nameof(SiteProperty.MaximumNumberOfGuest));
                b.Property(x => x.Livingrooms).HasColumnName(nameof(SiteProperty.Livingrooms));
                b.Property(x => x.PropertyDescription).HasColumnName(nameof(SiteProperty.PropertyDescription)).IsRequired();
                b.Property(x => x.HourseRules).HasColumnName(nameof(SiteProperty.HourseRules));
                b.Property(x => x.ImportantInformation).HasColumnName(nameof(SiteProperty.ImportantInformation));
                b.Property(x => x.Address).HasColumnName(nameof(SiteProperty.Address));
                b.Property(x => x.StreetAndBuildingNumber).HasColumnName(nameof(SiteProperty.StreetAndBuildingNumber));
                b.Property(x => x.LandMark).HasColumnName(nameof(SiteProperty.LandMark));
                b.Property(x => x.PricePerNight).HasColumnName(nameof(SiteProperty.PricePerNight));
                b.Property(x => x.IsActive).HasColumnName(nameof(SiteProperty.IsActive));
                b.HasOne<PropertyType>().WithMany().IsRequired().HasForeignKey(x => x.PropertyTypeId).OnDelete(DeleteBehavior.NoAction);
                b.HasOne<Governorate>().WithMany().IsRequired().HasForeignKey(x => x.GovernorateId).OnDelete(DeleteBehavior.NoAction);
                b.HasMany(x => x.PropertyFeatures).WithOne().HasForeignKey(x => x.SitePropertyId).IsRequired().OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<SitePropertyPropertyFeature>(b =>
        {
            b.ToTable(AhlanFeekumProConsts.DbTablePrefix + "SitePropertyPropertyFeature", AhlanFeekumProConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasKey(
                x => new { x.SitePropertyId, x.PropertyFeatureId }
            );

            b.HasOne<SiteProperty>().WithMany(x => x.PropertyFeatures).HasForeignKey(x => x.SitePropertyId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            b.HasOne<PropertyFeature>().WithMany().HasForeignKey(x => x.PropertyFeatureId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(
                    x => new { x.SitePropertyId, x.PropertyFeatureId }
            );
        });
        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<SpecialAdvertisment>(b =>
            {
                b.ToTable(AhlanFeekumProConsts.DbTablePrefix + "SpecialAdvertisments", AhlanFeekumProConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Image).HasColumnName(nameof(SpecialAdvertisment.Image)).IsRequired();
                b.Property(x => x.Order).HasColumnName(nameof(SpecialAdvertisment.Order));
                b.Property(x => x.IsActive).HasColumnName(nameof(SpecialAdvertisment.IsActive));
                b.HasOne<SiteProperty>().WithMany().IsRequired().HasForeignKey(x => x.SitePropertyId).OnDelete(DeleteBehavior.NoAction);
            });

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<OnlyForYouSection>(b =>
            {
                b.ToTable(AhlanFeekumProConsts.DbTablePrefix + "OnlyForYouSections", AhlanFeekumProConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.FirstPhoto).HasColumnName(nameof(OnlyForYouSection.FirstPhoto)).IsRequired();
                b.Property(x => x.SecondPhoto).HasColumnName(nameof(OnlyForYouSection.SecondPhoto)).IsRequired();
                b.Property(x => x.ThirdPhoto).HasColumnName(nameof(OnlyForYouSection.ThirdPhoto)).IsRequired();
            });

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {

        }
        if (builder.IsHostDatabase())
        {
            builder.Entity<PropertyCalendar>(b =>
            {
                b.ToTable(AhlanFeekumProConsts.DbTablePrefix + "PropertyCalendars", AhlanFeekumProConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Date).HasColumnName(nameof(PropertyCalendar.Date));
                b.Property(x => x.IsAvailable).HasColumnName(nameof(PropertyCalendar.IsAvailable));
                b.Property(x => x.Price).HasColumnName(nameof(PropertyCalendar.Price));
                b.Property(x => x.Note).HasColumnName(nameof(PropertyCalendar.Note));
                b.HasOne<SiteProperty>().WithMany().IsRequired().HasForeignKey(x => x.SitePropertyId).OnDelete(DeleteBehavior.NoAction);
            });

        }
    }
}