using Abp.Domain.Repositories;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Repositories
{
    public interface IPointConfigurationRepository : IRepository<PointConfigurations, Guid>
    {
        IQueryable<PointConfigurations> GetActivePointConfigurations();
    }
}
