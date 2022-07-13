using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.FLPDb;
using MPM.FLP.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.EntityFrameworkCore.Repositories
{
    public class ActivityLogRepository : FLPRepositoryBase<ActivityLogs, Guid>, IActivityLogRepository
    {
        private readonly IDbContextProvider<FLPDbContext> _dbContextProvider;
        public ActivityLogRepository(IDbContextProvider<FLPDbContext> dbContextProvider) : base(dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public async Task<Guid> InsertAndGetGuidAsync(ActivityLogs activityLogs)
        {
            FLPDbContext dbContext = _dbContextProvider.GetDbContext();
            dbContext.Add(activityLogs);
            await dbContext.SaveChangesAsync();

            return activityLogs.Id;
        }
    }
}
