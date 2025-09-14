using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AhlanFeekumPro.OnlyForYouSections
{
    public abstract class OnlyForYouSectionCreateDtoBase
    {
        public Guid FirstPhotoId { get; set; }
        public Guid SecondPhotoId { get; set; }
        public Guid ThirdPhotoId { get; set; }
    }
}