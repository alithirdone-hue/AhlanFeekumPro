using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AhlanFeekumPro.SiteProperties
{
    public abstract class SitePropertyCreateDtoBase
    {
        [Required]
        public string PropertyTitle { get; set; } = null!;
        public string? HotelName { get; set; }
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
        public bool IsActive { get; set; } = true;
        public Guid PropertyTypeId { get; set; }
        public Guid GovernorateId { get; set; }
        public List<Guid> PropertyFeatureIds { get; set; }
    }
}