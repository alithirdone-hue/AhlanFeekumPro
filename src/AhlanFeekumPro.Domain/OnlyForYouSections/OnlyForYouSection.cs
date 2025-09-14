using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace AhlanFeekumPro.OnlyForYouSections
{
    public abstract class OnlyForYouSectionBase : FullAuditedAggregateRoot<Guid>
    {
        public virtual Guid FirstPhotoId { get; set; }

        public virtual Guid SecondPhotoId { get; set; }

        public virtual Guid ThirdPhotoId { get; set; }

        protected OnlyForYouSectionBase()
        {

        }

        public OnlyForYouSectionBase(Guid id, Guid firstPhotoId, Guid secondPhotoId, Guid thirdPhotoId)
        {

            Id = id;
            FirstPhotoId = firstPhotoId;
            SecondPhotoId = secondPhotoId;
            ThirdPhotoId = thirdPhotoId;
        }

    }
}