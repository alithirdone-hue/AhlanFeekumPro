using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AhlanFeekumPro.PersonEvaluations
{
    public abstract class PersonEvaluationCreateDtoBase
    {
        [Required]
        [Range(PersonEvaluationConsts.RateMinLength, PersonEvaluationConsts.RateMaxLength)]
        public int Rate { get; set; }
        public string? Comment { get; set; }
        public Guid EvaluatorId { get; set; }
        public Guid EvaluatedPersonId { get; set; }
    }
}