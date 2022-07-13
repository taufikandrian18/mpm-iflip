﻿using Newtonsoft.Json;

namespace MPM.FLP.PushNotification
{
    public class GoogleNotification
    {
        public class DataPayload
        {
            // Add your custom properties as needed
            [JsonProperty("message")]
            public string Message { get; set; }
        }

        [JsonProperty("priority")]
        public string Priority { get; set; } = "high";

        [JsonProperty("data")]
        public DataPayload Data { get; set; }
    }
}
