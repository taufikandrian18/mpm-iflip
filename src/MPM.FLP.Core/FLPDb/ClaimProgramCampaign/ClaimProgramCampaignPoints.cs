using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class ClaimProgramCampaignPoints : EntityBase
    {
        public Guid GUIDClaimProgramCampaign { get; set; }
        public int EmployeeId { get; set; }
        public int Point { get; set; }
        //public virtual ICollection<ClaimProgramContentClaimers> ClaimProgramContentClaimers { get; set; }
        public virtual ICollection<ClaimProgramCampaignPrizeLogs> ClaimProgramCampaignPrizeLogs { get; set; }
        [JsonIgnore]
        public virtual ClaimProgramCampaigns ClaimProgramCampaigns { get; set; }
    }
}
