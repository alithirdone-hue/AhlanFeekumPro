using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace AhlanFeekumPro.SiteProperties
{
    public abstract class SitePropertyUpdateDtoBase : IHasConcurrencyStamp
    {
        [Required]
        public string PropertyTitle { get; set; } = null!;
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int NumberOfBed { get; set; }
        public int Floor { get; set; }
        public int MaximumNumberOfGuest { get; set; }
        public int Livingrooms { get; set; }
        [Required]
        public string PropertyDescription { get; set; } = null!;
        public string? HourseRules { get; set; }
        public string? ImportantInformation { get; set; }
        public string? Address { get; set; }
        public string? StreetAndBuildingNumber { get; set; }
        public string? LandMark { get; set; }
        public int PricePerNight { get; set; }
        public bool IsActive { get; set; }
        public Guid PropertyTypeId { get; set; }
        public List<Guid> PropertyFeatureIds { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}