using AhlanFeekumPro.UserProfiles;
using AhlanFeekumPro.SiteProperties;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace AhlanFeekumPro.PropertyEvaluations
{
    public abstract class PropertyEvaluationBase : FullAuditedAggregateRoot<Guid>
    {
        public virtual int Cleanliness { get; set; }

        public virtual int PriceAndValue { get; set; }

        public virtual int Location { get; set; }

        public virtual int Accuracy { get; set; }

        public virtual int Attitude { get; set; }

        [CanBeNull]
        public virtual string? RatingComment { get; set; }
        public Guid UserProfileId { get; set; }
        public Guid SitePropertyId { get; set; }

        protected PropertyEvaluationBase()
        {

        }

        public PropertyEvaluationBase(Guid id, Guid userProfileId, Guid sitePropertyId, int cleanliness, int priceAndValue, int location, int accuracy, int attitude, string? ratingComment = null)
        {

            Id = id;
            if (cleanliness < PropertyEvaluationConsts.CleanlinessMinLength)
            {
                throw new ArgumentOutOfRangeException(nameof(cleanliness), cleanliness, "The value of 'cleanliness' cannot be lower than " + PropertyEvaluationConsts.CleanlinessMinLength);
            }

            if (cleanliness > PropertyEvaluationConsts.CleanlinessMaxLength)
            {
                throw new ArgumentOutOfRangeException(nameof(cleanliness), cleanliness, "The value of 'cleanliness' cannot be greater than " + PropertyEvaluationConsts.CleanlinessMaxLength);
            }

            if (priceAndValue < PropertyEvaluationConsts.PriceAndValueMinLength)
            {
                throw new ArgumentOutOfRangeException(nameof(priceAndValue), priceAndValue, "The value of 'priceAndValue' cannot be lower than " + PropertyEvaluationConsts.PriceAndValueMinLength);
            }

            if (priceAndValue > PropertyEvaluationConsts.PriceAndValueMaxLength)
            {
                throw new ArgumentOutOfRangeException(nameof(priceAndValue), priceAndValue, "The value of 'priceAndValue' cannot be greater than " + PropertyEvaluationConsts.PriceAndValueMaxLength);
            }

            if (location < PropertyEvaluationConsts.LocationMinLength)
            {
                throw new ArgumentOutOfRangeException(nameof(location), location, "The value of 'location' cannot be lower than " + PropertyEvaluationConsts.LocationMinLength);
            }

            if (location > PropertyEvaluationConsts.LocationMaxLength)
            {
                throw new ArgumentOutOfRangeException(nameof(location), location, "The value of 'location' cannot be greater than " + PropertyEvaluationConsts.LocationMaxLength);
            }

            if (accuracy < PropertyEvaluationConsts.AccuracyMinLength)
            {
                throw new ArgumentOutOfRangeException(nameof(accuracy), accuracy, "The value of 'accuracy' cannot be lower than " + PropertyEvaluationConsts.AccuracyMinLength);
            }

            if (accuracy > PropertyEvaluationConsts.AccuracyMaxLength)
            {
                throw new ArgumentOutOfRangeException(nameof(accuracy), accuracy, "The value of 'accuracy' cannot be greater than " + PropertyEvaluationConsts.AccuracyMaxLength);
            }

            if (attitude < PropertyEvaluationConsts.AttitudeMinLength)
            {
                throw new ArgumentOutOfRangeException(nameof(attitude), attitude, "The value of 'attitude' cannot be lower than " + PropertyEvaluationConsts.AttitudeMinLength);
            }

            if (attitude > PropertyEvaluationConsts.AttitudeMaxLength)
            {
                throw new ArgumentOutOfRangeException(nameof(attitude), attitude, "The value of 'attitude' cannot be greater than " + PropertyEvaluationConsts.AttitudeMaxLength);
            }

            Cleanliness = cleanliness;
            PriceAndValue = priceAndValue;
            Location = location;
            Accuracy = accuracy;
            Attitude = attitude;
            RatingComment = ratingComment;
            UserProfileId = userProfileId;
            SitePropertyId = sitePropertyId;
        }

    }
}