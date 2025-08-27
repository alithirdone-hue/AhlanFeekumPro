using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace AhlanFeekumPro.PropertyEvaluations
{
    public abstract class PropertyEvaluationManagerBase : DomainService
    {
        protected IPropertyEvaluationRepository _propertyEvaluationRepository;

        public PropertyEvaluationManagerBase(IPropertyEvaluationRepository propertyEvaluationRepository)
        {
            _propertyEvaluationRepository = propertyEvaluationRepository;
        }

        public virtual async Task<PropertyEvaluation> CreateAsync(
        Guid userProfileId, Guid sitePropertyId, int cleanliness, int priceAndValue, int location, int accuracy, int attitude, string? ratingComment = null)
        {
            Check.NotNull(userProfileId, nameof(userProfileId));
            Check.NotNull(sitePropertyId, nameof(sitePropertyId));
            Check.Range(cleanliness, nameof(cleanliness), PropertyEvaluationConsts.CleanlinessMinLength, PropertyEvaluationConsts.CleanlinessMaxLength);
            Check.Range(priceAndValue, nameof(priceAndValue), PropertyEvaluationConsts.PriceAndValueMinLength, PropertyEvaluationConsts.PriceAndValueMaxLength);
            Check.Range(location, nameof(location), PropertyEvaluationConsts.LocationMinLength, PropertyEvaluationConsts.LocationMaxLength);
            Check.Range(accuracy, nameof(accuracy), PropertyEvaluationConsts.AccuracyMinLength, PropertyEvaluationConsts.AccuracyMaxLength);
            Check.Range(attitude, nameof(attitude), PropertyEvaluationConsts.AttitudeMinLength, PropertyEvaluationConsts.AttitudeMaxLength);

            var propertyEvaluation = new PropertyEvaluation(
             GuidGenerator.Create(),
             userProfileId, sitePropertyId, cleanliness, priceAndValue, location, accuracy, attitude, ratingComment
             );

            return await _propertyEvaluationRepository.InsertAsync(propertyEvaluation);
        }

        public virtual async Task<PropertyEvaluation> UpdateAsync(
            Guid id,
            Guid userProfileId, Guid sitePropertyId, int cleanliness, int priceAndValue, int location, int accuracy, int attitude, string? ratingComment = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(userProfileId, nameof(userProfileId));
            Check.NotNull(sitePropertyId, nameof(sitePropertyId));
            Check.Range(cleanliness, nameof(cleanliness), PropertyEvaluationConsts.CleanlinessMinLength, PropertyEvaluationConsts.CleanlinessMaxLength);
            Check.Range(priceAndValue, nameof(priceAndValue), PropertyEvaluationConsts.PriceAndValueMinLength, PropertyEvaluationConsts.PriceAndValueMaxLength);
            Check.Range(location, nameof(location), PropertyEvaluationConsts.LocationMinLength, PropertyEvaluationConsts.LocationMaxLength);
            Check.Range(accuracy, nameof(accuracy), PropertyEvaluationConsts.AccuracyMinLength, PropertyEvaluationConsts.AccuracyMaxLength);
            Check.Range(attitude, nameof(attitude), PropertyEvaluationConsts.AttitudeMinLength, PropertyEvaluationConsts.AttitudeMaxLength);

            var propertyEvaluation = await _propertyEvaluationRepository.GetAsync(id);

            propertyEvaluation.UserProfileId = userProfileId;
            propertyEvaluation.SitePropertyId = sitePropertyId;
            propertyEvaluation.Cleanliness = cleanliness;
            propertyEvaluation.PriceAndValue = priceAndValue;
            propertyEvaluation.Location = location;
            propertyEvaluation.Accuracy = accuracy;
            propertyEvaluation.Attitude = attitude;
            propertyEvaluation.RatingComment = ratingComment;

            propertyEvaluation.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _propertyEvaluationRepository.UpdateAsync(propertyEvaluation);
        }

    }
}