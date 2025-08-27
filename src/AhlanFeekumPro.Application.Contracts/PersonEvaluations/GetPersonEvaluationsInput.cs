using Volo.Abp.Application.Dtos;
using System;

namespace AhlanFeekumPro.PersonEvaluations
{
    public abstract class GetPersonEvaluationsInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public int? RateMin { get; set; }
        public int? RateMax { get; set; }
        public string? Comment { get; set; }
        public Guid? EvaluatorId { get; set; }
        public Guid? EvaluatedPersonId { get; set; }

        public GetPersonEvaluationsInputBase()
        {

        }
    }
}