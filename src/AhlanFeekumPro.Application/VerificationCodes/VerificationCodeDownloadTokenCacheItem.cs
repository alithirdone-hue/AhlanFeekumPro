using System;

namespace AhlanFeekumPro.VerificationCodes;

public abstract class VerificationCodeDownloadTokenCacheItemBase
{
    public string Token { get; set; } = null!;
}