using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.FLPDb;
using MPM.FLP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.EntityFrameworkCore.Repositories
{
    public class PointConfigurationRepository : FLPRepositoryBase<PointConfigurations, Guid>, IPointConfigurationRepository
    {
        private readonly IDbContextProvider<FLPDbContext> _dbContextProvider;
        public PointConfigurationRepository(IDbContextProvider<FLPDbContext> dbContextProvider) : base(dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public IQueryable<PointConfigurations> GetActivePointConfigurations()
        {
            IQueryable<PointConfigurations> pointConfigurations = _dbContextProvider.GetDbContext().PointConfigurations.FromSql(@"
                select *
                from (
                    select *,
                        ROW_NUMBER() OVER(PARTITION By ContentType, ActivityType order by IsDefault, EffDateFrom desc, EffDateFrom) RowNumber
                    from PointConfigurations
                    where IsDeleted = 0
                    and (IsDefault = 1 or (GETDATE() between EffDateFrom and EffDateTo))
                ) tblPointConfiguration
                where RowNumber = 1
                ");
            return pointConfigurations;
        }
    }
}
