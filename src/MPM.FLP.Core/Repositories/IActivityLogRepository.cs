using Abp.Domain.Repositories;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Repositories
{
    public interface IActivityLogRepository : IRepository<ActivityLogs, Guid>
    {
        Task<Guid> InsertAndGetGuidAsync(ActivityLogs activityLogs);
    }
}
