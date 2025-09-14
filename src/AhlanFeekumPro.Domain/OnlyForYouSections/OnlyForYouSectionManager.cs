using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace AhlanFeekumPro.OnlyForYouSections
{
    public abstract class OnlyForYouSectionManagerBase : DomainService
    {
        protected IOnlyForYouSectionRepository _onlyForYouSectionRepository;

        public OnlyForYouSectionManagerBase(IOnlyForYouSectionRepository onlyForYouSectionRepository)
        {
            _onlyForYouSectionRepository = onlyForYouSectionRepository;
        }

        public virtual async Task<OnlyForYouSection> CreateAsync(
        Guid firstPhotoId, Guid secondPhotoId, Guid thirdPhotoId)
        {

            var onlyForYouSection = new OnlyForYouSection(
             GuidGenerator.Create(),
             firstPhotoId, secondPhotoId, thirdPhotoId
             );

            return await _onlyForYouSectionRepository.InsertAsync(onlyForYouSection);
        }

        public virtual async Task<OnlyForYouSection> UpdateAsync(
            Guid id,
            Guid firstPhotoId, Guid secondPhotoId, Guid thirdPhotoId, [CanBeNull] string? concurrencyStamp = null
        )
        {

            var onlyForYouSection = await _onlyForYouSectionRepository.GetAsync(id);

            onlyForYouSection.FirstPhotoId = firstPhotoId;
            onlyForYouSection.SecondPhotoId = secondPhotoId;
            onlyForYouSection.ThirdPhotoId = thirdPhotoId;

            onlyForYouSection.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _onlyForYouSectionRepository.UpdateAsync(onlyForYouSection);
        }

    }
}