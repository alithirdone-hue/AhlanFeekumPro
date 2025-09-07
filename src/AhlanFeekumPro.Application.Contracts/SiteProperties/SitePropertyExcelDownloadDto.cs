using Volo.Abp.Application.Dtos;
using System;

namespace AhlanFeekumPro.SiteProperties
{
    public abstract class SitePropertyExcelDownloadDtoBase
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public string? PropertyTitle { get; set; }
        public string? HotelName { get; set; }
        public int? BedroomsMin { get; set; }
        public int? BedroomsMax { get; set; }
        public int? BathroomsMin { get; set; }
        public int? BathroomsMax { get; set; }
        public int? NumberOfBedMin { get; set; }
        public int? NumberOfBedMax { get; set; }
        public int? FloorMin { get; set; }
        public int? FloorMax { get; set; }
        public int? MaximumNumberOfGuestMin { get; set; }
        public int? MaximumNumberOfGuestMax { get; set; }
        public int? LivingroomsMin { get; set; }
        public int? LivingroomsMax { get; set; }
        public string? PropertyDescription { get; set; }
        public string? HourseRules { get; set; }
        public string? ImportantInformation { get; set; }
        public string? Address { get; set; }
        public string? StreetAndBuildingNumber { get; set; }
        public string? LandMark { get; set; }
        public int? PricePerNightMin { get; set; }
        public int? PricePerNightMax { get; set; }
        public bool? IsActive { get; set; }
        public Guid? PropertyTypeId { get; set; }
        public Guid? GovernorateId { get; set; }
        public Guid? PropertyFeatureId { get; set; }

        public SitePropertyExcelDownloadDtoBase()
        {

        }
    }
}