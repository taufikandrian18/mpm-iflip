using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IChannelAppService : IApplicationService
    {
        IQueryable<Channels> GetAll();
        void Create(Channels input);
        void Update(Channels input);
        void SoftDelete(Guid id, string username);
    }
}
