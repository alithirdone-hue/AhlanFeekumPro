using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace AhlanFeekumPro.Governorates
{
    public abstract class GovernorateManagerBase : DomainService
    {
        protected IGovernorateRepository _governorateRepository;

        public GovernorateManagerBase(IGovernorateRepository governorateRepository)
        {
            _governorateRepository = governorateRepository;
        }

        public virtual async Task<Governorate> CreateAsync(
        string title, int order, bool isActive)
        {
            Check.NotNullOrWhiteSpace(title, nameof(title));

            var governorate = new Governorate(
             GuidGenerator.Create(),
             title, order, isActive
             );

            return await _governorateRepository.InsertAsync(governorate);
        }

        public virtual async Task<Governorate> UpdateAsync(
            Guid id,
            string title, int order, bool isActive, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(title, nameof(title));

            var governorate = await _governorateRepository.GetAsync(id);

            governorate.Title = title;
            governorate.Order = order;
            governorate.IsActive = isActive;

            governorate.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _governorateRepository.UpdateAsync(governorate);
        }

    }
}