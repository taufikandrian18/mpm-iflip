using Abp.Application.Services;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IPushNotificationSubscriberAppService : IApplicationService
    {
        IQueryable<PushNotificationSubscribers> GetAllByUsername(string username);
        void Create(CreatePushNotificationSubscriberDto input);
        void Update(UpdatePushNotificationSubscriberDto input);
        void Delete(string deviceToken);
    }
}
