using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services.Dto
{
    public class CreatePushNotificationSubscriberDto
    {
        public string Username { get; set; }
        public string DeviceToken { get; set; }
    }

    public class UpdatePushNotificationSubscriberDto
    {
        public string OldDeviceToken { get; set; }
        public string NewDeviceToken { get; set; }
    }
}
