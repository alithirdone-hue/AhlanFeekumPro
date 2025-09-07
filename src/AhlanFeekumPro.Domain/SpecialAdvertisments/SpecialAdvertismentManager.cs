using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace AhlanFeekumPro.SpecialAdvertisments
{
    public abstract class SpecialAdvertismentManagerBase : DomainService
    {
        protected ISpecialAdvertismentRepository _specialAdvertismentRepository;

        public SpecialAdvertismentManagerBase(ISpecialAdvertismentRepository specialAdvertismentRepository)
        {
            _specialAdvertismentRepository = specialAdvertismentRepository;
        }

        public virtual async Task<SpecialAdvertisment> CreateAsync(
        Guid sitePropertyId, string image, int order, bool isActive)
        {
            Check.NotNull(sitePropertyId, nameof(sitePropertyId));
            Check.NotNullOrWhiteSpace(image, nameof(image));

            var specialAdvertisment = new SpecialAdvertisment(
             GuidGenerator.Create(),
             sitePropertyId, image, order, isActive
             );

            return await _specialAdvertismentRepository.InsertAsync(specialAdvertisment);
        }

        public virtual async Task<SpecialAdvertisment> UpdateAsync(
            Guid id,
            Guid sitePropertyId, string image, int order, bool isActive, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(sitePropertyId, nameof(sitePropertyId));
            Check.NotNullOrWhiteSpace(image, nameof(image));

            var specialAdvertisment = await _specialAdvertismentRepository.GetAsync(id);

            specialAdvertisment.SitePropertyId = sitePropertyId;
            specialAdvertisment.Image = image;
            specialAdvertisment.Order = order;
            specialAdvertisment.IsActive = isActive;

            specialAdvertisment.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _specialAdvertismentRepository.UpdateAsync(specialAdvertisment);
        }

    }
}