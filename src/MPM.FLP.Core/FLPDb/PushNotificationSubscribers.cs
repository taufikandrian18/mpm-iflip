using Abp.Domain.Entities;
using System;
namespace MPM.FLP.FLPDb
{
    public class PushNotificationSubscribers : Entity<Guid>
    {
        public override Guid Id { get; set; }
        public string Username { get; set; }
        public string DeviceToken { get; set; }
    }
}
