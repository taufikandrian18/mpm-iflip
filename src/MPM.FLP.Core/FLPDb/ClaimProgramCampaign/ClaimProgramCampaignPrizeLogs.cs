using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class ClaimProgramCampaignPrizeLogs : EntityBase
    {
        public Guid GUIDClaimProgramCampaignPoints { get; set; }
        public Guid GUIDClaimProgramCampaignPrizes { get; set; }
        
        [JsonIgnore]
        public virtual ClaimProgramCampaignPoints ClaimProgramCampaignPoints { get; set; }
        public virtual ClaimProgramCampaignPrizes ClaimProgramCampaignPrizes { get; set; }
    }
}
