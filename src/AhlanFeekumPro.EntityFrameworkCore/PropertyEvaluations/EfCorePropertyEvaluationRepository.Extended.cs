using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using AhlanFeekumPro.EntityFrameworkCore;

namespace AhlanFeekumPro.PropertyEvaluations
{
    public class EfCorePropertyEvaluationRepository : EfCorePropertyEvaluationRepositoryBase, IPropertyEvaluationRepository
    {
        public EfCorePropertyEvaluationRepository(IDbContextProvider<AhlanFeekumProDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}