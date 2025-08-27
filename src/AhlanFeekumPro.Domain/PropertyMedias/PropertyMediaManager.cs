using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace AhlanFeekumPro.PropertyMedias
{
    public abstract class PropertyMediaManagerBase : DomainService
    {
        protected IPropertyMediaRepository _propertyMediaRepository;

        public PropertyMediaManagerBase(IPropertyMediaRepository propertyMediaRepository)
        {
            _propertyMediaRepository = propertyMediaRepository;
        }

        public virtual async Task<PropertyMedia> CreateAsync(
        Guid sitePropertyId, string image, int order, bool isActive)
        {
            Check.NotNull(sitePropertyId, nameof(sitePropertyId));
            Check.NotNullOrWhiteSpace(image, nameof(image));

            var propertyMedia = new PropertyMedia(
             GuidGenerator.Create(),
             sitePropertyId, image, order, isActive
             );

            return await _propertyMediaRepository.InsertAsync(propertyMedia);
        }

        public virtual async Task<PropertyMedia> UpdateAsync(
            Guid id,
            Guid sitePropertyId, string image, int order, bool isActive, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(sitePropertyId, nameof(sitePropertyId));
            Check.NotNullOrWhiteSpace(image, nameof(image));

            var propertyMedia = await _propertyMediaRepository.GetAsync(id);

            propertyMedia.SitePropertyId = sitePropertyId;
            propertyMedia.Image = image;
            propertyMedia.Order = order;
            propertyMedia.isActive = isActive;

            propertyMedia.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _propertyMediaRepository.UpdateAsync(propertyMedia);
        }

    }
}