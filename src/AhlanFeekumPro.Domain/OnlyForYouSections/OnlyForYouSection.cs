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
        [NotNull]
        public virtual string FirstPhoto { get; set; }

        [NotNull]
        public virtual string SecondPhoto { get; set; }

        [NotNull]
        public virtual string ThirdPhoto { get; set; }

        protected OnlyForYouSectionBase()
        {

        }

        public OnlyForYouSectionBase(Guid id, string firstPhoto, string secondPhoto, string thirdPhoto)
        {

            Id = id;
            Check.NotNull(firstPhoto, nameof(firstPhoto));
            Check.NotNull(secondPhoto, nameof(secondPhoto));
            Check.NotNull(thirdPhoto, nameof(thirdPhoto));
            FirstPhoto = firstPhoto;
            SecondPhoto = secondPhoto;
            ThirdPhoto = thirdPhoto;
        }

    }
}