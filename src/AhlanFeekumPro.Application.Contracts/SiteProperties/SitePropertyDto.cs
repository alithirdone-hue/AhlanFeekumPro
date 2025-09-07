using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace AhlanFeekumPro.SiteProperties
{
    public abstract class SitePropertyDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string PropertyTitle { get; set; } = null!;
        public string? HotelName { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int NumberOfBed { get; set; }
        public int Floor { get; set; }
        public int MaximumNumberOfGuest { get; set; }
        public int Livingrooms { get; set; }
        public string PropertyDescription { get; set; } = null!;
        public string? HourseRules { get; set; }
        public string? ImportantInformation { get; set; }
        public string? Address { get; set; }
        public string? StreetAndBuildingNumber { get; set; }
        public string? LandMark { get; set; }
        public int PricePerNight { get; set; }
        public bool IsActive { get; set; }
        public Guid PropertyTypeId { get; set; }
        public Guid GovernorateId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}