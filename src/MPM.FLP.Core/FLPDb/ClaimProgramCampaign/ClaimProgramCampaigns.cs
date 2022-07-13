using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class ClaimProgramCampaigns : EntityBase
    {
        public string Name { get; set; }
        public string Description {get;set;}
        public bool? IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public virtual ICollection<ClaimProgramCampaignPoints> ClaimProgramCampaignPoints { get; set; }
        public virtual ICollection<ClaimProgramCampaignPrizes> ClaimProgramCampaignPrizes { get; set; }
    }
}
