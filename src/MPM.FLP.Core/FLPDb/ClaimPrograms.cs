using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class ClaimPrograms : Entity<Guid>
    {
        public ClaimPrograms()
        {
            ClaimProgramAttachments = new HashSet<ClaimProgramAttachments>();
            ClaimProgramClaimers = new HashSet<ClaimProgramClaimers>();
        }

        public override Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public bool IsPublished { get; set; }
        public string FeaturedImageUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsDoku { get; set; }
        public decimal? DokuReward { get; set; }
        public string NonDokuReward { get; set; }
        public bool? IsH3 { get; set; }
        public bool? IsH3Ahass { get; set; }


        public virtual ICollection<ClaimProgramAttachments> ClaimProgramAttachments { get; set; }
        public virtual ICollection<ClaimProgramClaimers> ClaimProgramClaimers { get; set; }
    }
}
