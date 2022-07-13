using Abp.Authorization;
using Abp.Domain.Repositories;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class WebNotificationAppService : FLPAppServiceBase, IWebNotificationAppService
    {
        private readonly IRepository<WebNotifications, Guid> _webNotificationRepository;

        public WebNotificationAppService(IRepository<WebNotifications, Guid> webNotificationRepository)
        {
            _webNotificationRepository = webNotificationRepository;
        }

        public IQueryable<WebNotifications> GetAll()
        {
            return _webNotificationRepository.GetAll();
        }

        public void SetRead(Guid id)
        {
            var webNotification = _webNotificationRepository.GetAll().FirstOrDefault(x => x.Id == id);
            webNotification.IsRead = true;
            webNotification.LastModifierUsername = webNotification.ReceiverUsername;
            webNotification.LastModificationTime = DateTime.UtcNow.AddHours(7);
            _webNotificationRepository.Update(webNotification);
        }
    }
}
