using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MPM.FLP.FLPDb
{
    public class ContentBankAssignees : EntityBase
    {
        public Guid GUIDContentBankDetail { get; set; }
        public long GUIDEmployee {get;set;}
        public string KodeDealerMPM { get; set; }
        public int Status { get; set; }

        [JsonIgnore]
        public virtual ContentBankDetails ContentBankDetails { get; set; }
        public virtual List<ContentBankAssigneeProofs> ContentBankAssigneeProofs { get; set; }
    }
}
