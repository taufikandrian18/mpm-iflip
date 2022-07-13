using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class ClaimProgramClaimers : Entity<Guid>
    {
        public override Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string ClaimerUsername { get; set; }
        public string StorageUrl { get; set; }
        public Guid ClaimProgramId { get; set; }
        public bool? IsApproved { get; set; }
        public string OTP { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? VerifiedTime { get; set; }

        [JsonIgnore]
        public virtual ClaimPrograms ClaimPrograms { get; set; }
    }
}
