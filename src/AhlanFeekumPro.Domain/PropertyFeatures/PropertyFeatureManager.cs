using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace AhlanFeekumPro.PropertyFeatures
{
    public abstract class PropertyFeatureManagerBase : DomainService
    {
        protected IPropertyFeatureRepository _propertyFeatureRepository;

        public PropertyFeatureManagerBase(IPropertyFeatureRepository propertyFeatureRepository)
        {
            _propertyFeatureRepository = propertyFeatureRepository;
        }

        public virtual async Task<PropertyFeature> CreateAsync(
        string title, string icon, int order, bool isActive)
        {
            Check.NotNullOrWhiteSpace(title, nameof(title));
            Check.NotNullOrWhiteSpace(icon, nameof(icon));

            var propertyFeature = new PropertyFeature(
             GuidGenerator.Create(),
             title, icon, order, isActive
             );

            return await _propertyFeatureRepository.InsertAsync(propertyFeature);
        }

        public virtual async Task<PropertyFeature> UpdateAsync(
            Guid id,
            string title, string icon, int order, bool isActive, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(title, nameof(title));
            Check.NotNullOrWhiteSpace(icon, nameof(icon));

            var propertyFeature = await _propertyFeatureRepository.GetAsync(id);

            propertyFeature.Title = title;
            propertyFeature.Icon = icon;
            propertyFeature.Order = order;
            propertyFeature.IsActive = isActive;

            propertyFeature.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _propertyFeatureRepository.UpdateAsync(propertyFeature);
        }

    }
}