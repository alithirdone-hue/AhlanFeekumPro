using AhlanFeekumPro.PropertyTypes;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace AhlanFeekumPro.SiteProperties
{
    public abstract class SitePropertyBase : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string PropertyTitle { get; set; }

        public virtual int Bedrooms { get; set; }

        public virtual int Bathrooms { get; set; }

        public virtual int NumberOfBed { get; set; }

        public virtual int Floor { get; set; }

        public virtual int MaximumNumberOfGuest { get; set; }

        public virtual int Livingrooms { get; set; }

        [NotNull]
        public virtual string PropertyDescription { get; set; }

        [CanBeNull]
        public virtual string? HourseRules { get; set; }

        [CanBeNull]
        public virtual string? ImportantInformation { get; set; }

        [CanBeNull]
        public virtual string? Address { get; set; }

        [CanBeNull]
        public virtual string? StreetAndBuildingNumber { get; set; }

        [CanBeNull]
        public virtual string? LandMark { get; set; }

        public virtual int PricePerNight { get; set; }

        public virtual bool IsActive { get; set; }
        public Guid PropertyTypeId { get; set; }
        public ICollection<SitePropertyPropertyFeature> PropertyFeatures { get; private set; }

        protected SitePropertyBase()
        {

        }

        public SitePropertyBase(Guid id, Guid propertyTypeId, string propertyTitle, int bedrooms, int bathrooms, int numberOfBed, int floor, int maximumNumberOfGuest, int livingrooms, string propertyDescription, int pricePerNight, bool isActive, string? hourseRules = null, string? importantInformation = null, string? address = null, string? streetAndBuildingNumber = null, string? landMark = null)
        {

            Id = id;
            Check.NotNull(propertyTitle, nameof(propertyTitle));
            Check.NotNull(propertyDescription, nameof(propertyDescription));
            PropertyTitle = propertyTitle;
            Bedrooms = bedrooms;
            Bathrooms = bathrooms;
            NumberOfBed = numberOfBed;
            Floor = floor;
            MaximumNumberOfGuest = maximumNumberOfGuest;
            Livingrooms = livingrooms;
            PropertyDescription = propertyDescription;
            PricePerNight = pricePerNight;
            IsActive = isActive;
            HourseRules = hourseRules;
            ImportantInformation = importantInformation;
            Address = address;
            StreetAndBuildingNumber = streetAndBuildingNumber;
            LandMark = landMark;
            PropertyTypeId = propertyTypeId;
            PropertyFeatures = new Collection<SitePropertyPropertyFeature>();
        }
        public virtual void AddPropertyFeature(Guid propertyFeatureId)
        {
            Check.NotNull(propertyFeatureId, nameof(propertyFeatureId));

            if (IsInPropertyFeatures(propertyFeatureId))
            {
                return;
            }

            PropertyFeatures.Add(new SitePropertyPropertyFeature(Id, propertyFeatureId));
        }

        public virtual void RemovePropertyFeature(Guid propertyFeatureId)
        {
            Check.NotNull(propertyFeatureId, nameof(propertyFeatureId));

            if (!IsInPropertyFeatures(propertyFeatureId))
            {
                return;
            }

            PropertyFeatures.RemoveAll(x => x.PropertyFeatureId == propertyFeatureId);
        }

        public virtual void RemoveAllPropertyFeaturesExceptGivenIds(List<Guid> propertyFeatureIds)
        {
            Check.NotNullOrEmpty(propertyFeatureIds, nameof(propertyFeatureIds));

            PropertyFeatures.RemoveAll(x => !propertyFeatureIds.Contains(x.PropertyFeatureId));
        }

        public virtual void RemoveAllPropertyFeatures()
        {
            PropertyFeatures.RemoveAll(x => x.SitePropertyId == Id);
        }

        private bool IsInPropertyFeatures(Guid propertyFeatureId)
        {
            return PropertyFeatures.Any(x => x.PropertyFeatureId == propertyFeatureId);
        }
    }
}