using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class SalesIncentivePrograms : Entity<Guid>
    {
        public SalesIncentivePrograms()
        {
            SalesIncentiveProgramAttachments = new HashSet<SalesIncentiveProgramAttachments>();
            SalesIncentiveProgramJabatans = new HashSet<SalesIncentiveProgramJabatans>();
            SalesIncentiveProgramKotas = new HashSet<SalesIncentiveProgramKotas>();
            SalesIncentiveProgramTarget = new HashSet<SalesIncentiveProgramTarget>();
            SalesIncentiveProgramAssignee = new HashSet<SalesIncentiveProgramAssignee>();
        }

        public override Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public bool IsPublished { get; set; }
        public string FeaturedImageUrl { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        //public string TipeMotor { get; set; }
        public Guid ProductTypesId { get; set; }
        public decimal Incentive { get; set; }
        public int? TipePembayaran { get; set; }
        public int? ReadingTime { get; set; }
        public long ViewCount { get; set; }

        public virtual ICollection<SalesIncentiveProgramAttachments> SalesIncentiveProgramAttachments { get; set; }
        public virtual ICollection<SalesIncentiveProgramJabatans> SalesIncentiveProgramJabatans { get; set; }
        public virtual ICollection<SalesIncentiveProgramKotas> SalesIncentiveProgramKotas { get; set; }
        public virtual ICollection<SalesIncentiveProgramTarget> SalesIncentiveProgramTarget { get; set; }
        public virtual ICollection<SalesIncentiveProgramAssignee> SalesIncentiveProgramAssignee { get; set; }
        [JsonIgnore]
        public virtual ProductTypes ProductTypes { get; set; }
    }
}
