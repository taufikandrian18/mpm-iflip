using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class ClaimProgramCampaignPrizes : EntityBase
    {
        public Guid GUIDClaimProgramCampaign { get; set; }
        public string Name {get;set;}
        public string Description { get; set; }
        public string Prize { get; set; }
        public int RedeemPoint {get;set;}
        public string AttachmentURL { get; set; }
        //public virtual ICollection<ClaimProgramContentClaimers> ClaimProgramContentClaimers { get; set; }
        public virtual ICollection<ClaimProgramCampaignPrizeLogs> ClaimProgramCampaignPrizeLogs { get; set; }
        [JsonIgnore]
        public virtual ClaimProgramCampaigns ClaimProgramCampaigns { get; set; }
    }
}
