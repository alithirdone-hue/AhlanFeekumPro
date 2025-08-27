using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace AhlanFeekumPro.PropertyTypes
{
    public abstract class PropertyTypeManagerBase : DomainService
    {
        protected IPropertyTypeRepository _propertyTypeRepository;

        public PropertyTypeManagerBase(IPropertyTypeRepository propertyTypeRepository)
        {
            _propertyTypeRepository = propertyTypeRepository;
        }

        public virtual async Task<PropertyType> CreateAsync(
        string title, int order, bool isActive)
        {
            Check.NotNullOrWhiteSpace(title, nameof(title));

            var propertyType = new PropertyType(
             GuidGenerator.Create(),
             title, order, isActive
             );

            return await _propertyTypeRepository.InsertAsync(propertyType);
        }

        public virtual async Task<PropertyType> UpdateAsync(
            Guid id,
            string title, int order, bool isActive, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(title, nameof(title));

            var propertyType = await _propertyTypeRepository.GetAsync(id);

            propertyType.Title = title;
            propertyType.Order = order;
            propertyType.IsActive = isActive;

            propertyType.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _propertyTypeRepository.UpdateAsync(propertyType);
        }

    }
}