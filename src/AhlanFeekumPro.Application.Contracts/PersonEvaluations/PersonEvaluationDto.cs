using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace AhlanFeekumPro.PersonEvaluations
{
    public abstract class PersonEvaluationDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public int Rate { get; set; }
        public string? Comment { get; set; }
        public Guid EvaluatorId { get; set; }
        public Guid EvaluatedPersonId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}