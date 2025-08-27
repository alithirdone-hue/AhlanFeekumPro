using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace AhlanFeekumPro.VerificationCodes
{
    public abstract class VerificationCodeManagerBase : DomainService
    {
        protected IVerificationCodeRepository _verificationCodeRepository;

        public VerificationCodeManagerBase(IVerificationCodeRepository verificationCodeRepository)
        {
            _verificationCodeRepository = verificationCodeRepository;
        }

        public virtual async Task<VerificationCode> CreateAsync(
        string phoneOrEmail, int securityCode, bool isExpired)
        {
            Check.NotNullOrWhiteSpace(phoneOrEmail, nameof(phoneOrEmail));

            var verificationCode = new VerificationCode(
             GuidGenerator.Create(),
             phoneOrEmail, securityCode, isExpired
             );

            return await _verificationCodeRepository.InsertAsync(verificationCode);
        }

        public virtual async Task<VerificationCode> UpdateAsync(
            Guid id,
            string phoneOrEmail, int securityCode, bool isExpired, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(phoneOrEmail, nameof(phoneOrEmail));

            var verificationCode = await _verificationCodeRepository.GetAsync(id);

            verificationCode.PhoneOrEmail = phoneOrEmail;
            verificationCode.SecurityCode = securityCode;
            verificationCode.IsExpired = isExpired;

            verificationCode.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _verificationCodeRepository.UpdateAsync(verificationCode);
        }

    }
}