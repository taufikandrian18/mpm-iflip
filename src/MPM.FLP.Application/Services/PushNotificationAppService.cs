using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Authorization;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class PushNotificationSubscriberAppService : FLPAppServiceBase, IPushNotificationSubscriberAppService
    {
        private readonly IRepository<PushNotificationSubscribers, Guid> _pushNotificationSubscriberRepository;

        public PushNotificationSubscriberAppService(IRepository<PushNotificationSubscribers, Guid> pushNotificationSubscriberRepository)
        {
            _pushNotificationSubscriberRepository = pushNotificationSubscriberRepository;
        }
        public void Create(CreatePushNotificationSubscriberDto input)
        {
            PushNotificationSubscribers subscriber = new PushNotificationSubscribers() 
            {
                Id = Guid.NewGuid(),
                Username = input.Username,
                DeviceToken = input.DeviceToken
            };
            _pushNotificationSubscriberRepository.Insert(subscriber);
        }

        public void Update(UpdatePushNotificationSubscriberDto input) 
        {
            var subscriber = _pushNotificationSubscriberRepository.GetAll().FirstOrDefault(x => x.DeviceToken == input.OldDeviceToken);
            if (subscriber != null) 
            {
                subscriber.DeviceToken = input.NewDeviceToken;
                _pushNotificationSubscriberRepository.Update(subscriber);
            }
        }

        public IQueryable<PushNotificationSubscribers> GetAllByUsername(string username)
        {
            return _pushNotificationSubscriberRepository.GetAll().Where(x => x.Username == username);
        }

        public void Delete(string deviceToken)
        {
            var pushNotification = _pushNotificationSubscriberRepository.GetAll().FirstOrDefault(x => x.DeviceToken == deviceToken);
            _pushNotificationSubscriberRepository.Delete(pushNotification);
        }
    }
}
