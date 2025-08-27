using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace AhlanFeekumPro.MobileResponses
{
    public  class RegisterResponse
    {
        public Guid Id { get; set; }
        [NotNull]
        public virtual string Name { get; set; }

        [CanBeNull]
        public virtual string? Email { get; set; }

        [CanBeNull]
        public virtual string? PhoneNumber { get; set; }

        [CanBeNull]
        public virtual string? Latitude { get; set; }

        [CanBeNull]
        public virtual string? Longitude { get; set; }

        [CanBeNull]
        public virtual string? Address { get; set; }

        [CanBeNull]
        public virtual string? ProfilePhoto { get; set; }

        public virtual bool IsSuperHost { get; set; }

        public string? RoleId { get; set; } = null;

    }
}
