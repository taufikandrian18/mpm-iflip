using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class LogActivityDetails : EntityBase
    {
        public Guid LogActivityGUID { get; set; }
        public Guid RelatedId { get; set; }
        public string RelatedTitle { get; set; }
        public string JsonDataBefore { get; set; }
        public string JsonDataAfter { get; set; }

        [JsonIgnore]
        public virtual LogActivities LogActivities { get; set; }
    }
}
