using AhlanFeekumPro.VerificationCodes;
using AhlanFeekumPro.PropertyMedias;
using AhlanFeekumPro.PropertyEvaluations;
using AhlanFeekumPro.PersonEvaluations;
using AhlanFeekumPro.FavoriteProperties;
using AhlanFeekumPro.SiteProperties;
using AhlanFeekumPro.PropertyTypes;
using AhlanFeekumPro.PropertyFeatures;
using AhlanFeekumPro.UserProfiles;
using Volo.Abp.AutoMapper;
using AutoMapper;

namespace AhlanFeekumPro.Blazor;

public class AhlanFeekumProBlazorAutoMapperProfile : Profile
{
    public AhlanFeekumProBlazorAutoMapperProfile()
    {
        //Define your AutoMapper configuration here for the Blazor project.

        CreateMap<UserProfileDto, UserProfileUpdateDto>();

        CreateMap<PropertyFeatureDto, PropertyFeatureUpdateDto>();

        CreateMap<PropertyTypeDto, PropertyTypeUpdateDto>();

        CreateMap<SitePropertyDto, SitePropertyUpdateDto>().Ignore(x => x.PropertyFeatureIds);

        CreateMap<FavoritePropertyDto, FavoritePropertyUpdateDto>();

        CreateMap<PersonEvaluationDto, PersonEvaluationUpdateDto>();

        CreateMap<PropertyEvaluationDto, PropertyEvaluationUpdateDto>();

        CreateMap<PropertyMediaDto, PropertyMediaUpdateDto>();

        CreateMap<VerificationCodeDto, VerificationCodeUpdateDto>();
    }
}