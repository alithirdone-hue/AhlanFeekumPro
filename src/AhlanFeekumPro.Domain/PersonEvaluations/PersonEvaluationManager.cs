using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace AhlanFeekumPro.PersonEvaluations
{
    public abstract class PersonEvaluationManagerBase : DomainService
    {
        protected IPersonEvaluationRepository _personEvaluationRepository;

        public PersonEvaluationManagerBase(IPersonEvaluationRepository personEvaluationRepository)
        {
            _personEvaluationRepository = personEvaluationRepository;
        }

        public virtual async Task<PersonEvaluation> CreateAsync(
        Guid evaluatorId, Guid evaluatedPersonId, int rate, string? comment = null)
        {
            Check.NotNull(evaluatorId, nameof(evaluatorId));
            Check.NotNull(evaluatedPersonId, nameof(evaluatedPersonId));
            Check.Range(rate, nameof(rate), PersonEvaluationConsts.RateMinLength, PersonEvaluationConsts.RateMaxLength);

            var personEvaluation = new PersonEvaluation(
             GuidGenerator.Create(),
             evaluatorId, evaluatedPersonId, rate, comment
             );

            return await _personEvaluationRepository.InsertAsync(personEvaluation);
        }

        public virtual async Task<PersonEvaluation> UpdateAsync(
            Guid id,
            Guid evaluatorId, Guid evaluatedPersonId, int rate, string? comment = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNull(evaluatorId, nameof(evaluatorId));
            Check.NotNull(evaluatedPersonId, nameof(evaluatedPersonId));
            Check.Range(rate, nameof(rate), PersonEvaluationConsts.RateMinLength, PersonEvaluationConsts.RateMaxLength);

            var personEvaluation = await _personEvaluationRepository.GetAsync(id);

            personEvaluation.EvaluatorId = evaluatorId;
            personEvaluation.EvaluatedPersonId = evaluatedPersonId;
            personEvaluation.Rate = rate;
            personEvaluation.Comment = comment;

            personEvaluation.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _personEvaluationRepository.UpdateAsync(personEvaluation);
        }

    }
}