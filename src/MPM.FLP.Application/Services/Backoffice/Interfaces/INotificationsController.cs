using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

namespace MPM.FLP.Services.Backoffice
{
    public interface INotificationsController : IApplicationService
    {
        List<WebNotifications> GetNotifications();
        String SetRead(Guid notiId);
    }
}