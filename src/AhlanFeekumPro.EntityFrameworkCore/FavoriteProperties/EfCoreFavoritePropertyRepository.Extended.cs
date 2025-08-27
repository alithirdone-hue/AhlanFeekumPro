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

namespace AhlanFeekumPro.FavoriteProperties
{
    public class EfCoreFavoritePropertyRepository : EfCoreFavoritePropertyRepositoryBase, IFavoritePropertyRepository
    {
        public EfCoreFavoritePropertyRepository(IDbContextProvider<AhlanFeekumProDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}