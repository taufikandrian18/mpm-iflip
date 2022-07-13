using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class LogActivities : EntityBase
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string PageName { get; set; }
        public string Action { get; set; }
        
        public virtual LogActivityDetails LogActivityDetails { get; set; }
    }
}
