using AhlanFeekumPro.UserProfiles;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace AhlanFeekumPro.PersonEvaluations
{
    public abstract class PersonEvaluationBase : FullAuditedAggregateRoot<Guid>
    {
        public virtual int Rate { get; set; }

        [CanBeNull]
        public virtual string? Comment { get; set; }
        public Guid EvaluatorId { get; set; }
        public Guid EvaluatedPersonId { get; set; }

        protected PersonEvaluationBase()
        {

        }

        public PersonEvaluationBase(Guid id, Guid evaluatorId, Guid evaluatedPersonId, int rate, string? comment = null)
        {

            Id = id;
            if (rate < PersonEvaluationConsts.RateMinLength)
            {
                throw new ArgumentOutOfRangeException(nameof(rate), rate, "The value of 'rate' cannot be lower than " + PersonEvaluationConsts.RateMinLength);
            }

            if (rate > PersonEvaluationConsts.RateMaxLength)
            {
                throw new ArgumentOutOfRangeException(nameof(rate), rate, "The value of 'rate' cannot be greater than " + PersonEvaluationConsts.RateMaxLength);
            }

            Rate = rate;
            Comment = comment;
            EvaluatorId = evaluatorId;
            EvaluatedPersonId = evaluatedPersonId;
        }

    }
}