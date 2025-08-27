using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace AhlanFeekumPro.FavoriteProperties
{
    public abstract class FavoritePropertyManagerBase : DomainService
    {
        protected IFavoritePropertyRepository _favoritePropertyRepository;

        public FavoritePropertyManagerBase(IFavoritePropertyRepository favoritePropertyRepository)
        {
            _favoritePropertyRepository = favoritePropertyRepository;
        }

        public virtual async Task<FavoriteProperty> CreateAsync(
        Guid userProfileId, Guid sitePropertyId)
        {
            Check.NotNull(userProfileId, nameof(userProfileId));
            Check.NotNull(sitePropertyId, nameof(sitePropertyId));

            var favoriteProperty = new FavoriteProperty(
             GuidGenerator.Create(),
             userProfileId, sitePropertyId
             );

            return await _favoritePropertyRepository.InsertAsync(favoriteProperty);
        }

        public virtual async Task<FavoriteProperty> UpdateAsync(
            Guid id,
            Guid userProfileId, Guid sitePropertyId, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(userProfileId, nameof(userProfileId));
            Check.NotNull(sitePropertyId, nameof(sitePropertyId));

            var favoriteProperty = await _favoritePropertyRepository.GetAsync(id);

            favoriteProperty.UserProfileId = userProfileId;
            favoriteProperty.SitePropertyId = sitePropertyId;

            favoriteProperty.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _favoritePropertyRepository.UpdateAsync(favoriteProperty);
        }

    }
}