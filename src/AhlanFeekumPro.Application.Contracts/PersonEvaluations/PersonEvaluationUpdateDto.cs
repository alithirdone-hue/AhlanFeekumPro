using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace AhlanFeekumPro.PersonEvaluations
{
    public abstract class PersonEvaluationUpdateDtoBase : IHasConcurrencyStamp
    {
        [Required]
        [Range(PersonEvaluationConsts.RateMinLength, PersonEvaluationConsts.RateMaxLength)]
        public int Rate { get; set; }
        public string? Comment { get; set; }
        public Guid EvaluatorId { get; set; }
        public Guid EvaluatedPersonId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}