using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class ClaimProgramContentClaimers : EntityBase
    {
        public Guid GUIDClaimProgramPrize { get; set; }
        public Guid GUIDClaimProgramContent { get; set; }
        public int EmployeeId {get;set;}
        public Guid GUIDClaimProgramPoint { get; set; }
        public string StorageUrl { get; set; }
        public string OTP { get; set; }
        public bool IsApprove { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? VerifiedTime { get; set; }
        public int EnumStatus { get; set; }
        [JsonIgnore]
        public virtual ClaimProgramContents ClaimProgramContents { get; set; }
        //public virtual ClaimProgramCampaignPrizes ClaimProgramCampaignPrizes { get; set; }
        //public virtual ClaimProgramCampaignPoints ClaimProgramCampaignPoints { get; set; }
    }
}
