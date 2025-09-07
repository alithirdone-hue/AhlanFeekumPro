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
        string firstPhoto, string secondPhoto, string thirdPhoto)
        {
            Check.NotNullOrWhiteSpace(firstPhoto, nameof(firstPhoto));
            Check.NotNullOrWhiteSpace(secondPhoto, nameof(secondPhoto));
            Check.NotNullOrWhiteSpace(thirdPhoto, nameof(thirdPhoto));

            var onlyForYouSection = new OnlyForYouSection(
             GuidGenerator.Create(),
             firstPhoto, secondPhoto, thirdPhoto
             );

            return await _onlyForYouSectionRepository.InsertAsync(onlyForYouSection);
        }

        public virtual async Task<OnlyForYouSection> UpdateAsync(
            Guid id,
            string firstPhoto, string secondPhoto, string thirdPhoto, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(firstPhoto, nameof(firstPhoto));
            Check.NotNullOrWhiteSpace(secondPhoto, nameof(secondPhoto));
            Check.NotNullOrWhiteSpace(thirdPhoto, nameof(thirdPhoto));

            var onlyForYouSection = await _onlyForYouSectionRepository.GetAsync(id);

            onlyForYouSection.FirstPhoto = firstPhoto;
            onlyForYouSection.SecondPhoto = secondPhoto;
            onlyForYouSection.ThirdPhoto = thirdPhoto;

            onlyForYouSection.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _onlyForYouSectionRepository.UpdateAsync(onlyForYouSection);
        }

    }
}