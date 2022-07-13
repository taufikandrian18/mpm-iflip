using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IWebNotificationAppService : IApplicationService
    {
        IQueryable<WebNotifications> GetAll();
        void SetRead(Guid id);
    }
}
