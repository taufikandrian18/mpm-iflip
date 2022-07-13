using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services.Dto
{
    public class ActivitiesInternalUsersDto
    {
        public string Name { get; set; }
        public string KodeDealer { get; set; }
        public string NamaDealer { get; set; }
        public string Kota { get; set; }
        public string IDMPM { get; set; }
        public List<ActivityLogs> Activity { get; set; }
    }

    public class ActivitiesDto { 
        public string ContentType { get; set; }
        public string ActivityType { get; set; }
        public string ContentTitle { get; set; }
        public string Username { get; set; } 
        public DateTime Time { get; set; }
    }
}
