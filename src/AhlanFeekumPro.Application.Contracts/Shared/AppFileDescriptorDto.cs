using System;
using Volo.Abp.Application.Dtos;

namespace AhlanFeekumPro.Shared;

public class AppFileDescriptorDto : EntityDto<Guid>
{
    public string Name { get; set; } = null!;

    public string MimeType { get; set; } = null!;
}