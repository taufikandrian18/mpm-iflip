using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MPM.FLP.FLPDb;
using System;
using System.Linq;

namespace MPM.FLP.Services.Backoffice
{
    public class NotificationsController : FLPAppServiceBase, INotificationsController
    {
        private readonly WebNotificationAppService _appService;

        public NotificationsController(WebNotificationAppService appService)
        {
            _appService = appService;
        }

        [HttpGet("/api/services/app/backoffice/Notifications/getAll")]
        public List<WebNotifications> GetNotifications()
        {
            List<WebNotifications> notifications = new List<WebNotifications>();
            string paramUserName = "admin";
            /*var userName = _userManager.Users.FirstOrDefault(y => y.Id == this.User.Identity.GetUserId());
            if (userName == null)
            {
                paramUserName = "admin";
            }
            else {
                paramUserName = userName.UserName;
            }
            paramUserName = userName.UserName;*/
            notifications = _appService.GetAll().Where(x=> x.ReceiverUsername == paramUserName).OrderBy(x=>x.IsRead).ToList();
            return notifications;
        }

        [HttpPost("/api/services/app/backoffice/Notifications/markAsRead")]
        public String SetRead(Guid notiId)
        {
            _appService.SetRead(notiId);
            return "Success read";
        }
    }
}