using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class ClaimProgramContents : EntityBase
    {
        public Guid GUIDClaimProgramCampaign { get; set; }
        public string  Name {get;set;}
        public string Description { get; set; }
        public string KetentuanPoin { get; set; }
        public int Point { get; set; }
        public int MaximumClaim { get; set; }
        public bool IsOtp { get; set; }
        public bool IsH1 {get;set;}
        public bool IsH2 {get;set;}
        public bool IsH3 {get;set;}
        public bool IsTBSM { get;set;}
        public bool IsPublished {get;set;}
        public DateTime? StartDate {get;set;}
        public DateTime? EndDate {get;set;}
        public virtual ICollection<ClaimProgramContentAttachments> ClaimProgramContentAttachments { get; set; }
        public virtual ICollection<ClaimProgramAssignees> ClaimProgramAssignees { get; set; }
        public virtual ICollection<ClaimProgramContentClaimers> ClaimProgramContentClaimers { get; set; }
    }
}
