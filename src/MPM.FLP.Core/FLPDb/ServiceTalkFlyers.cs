using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class ServiceTalkFlyers : Entity<Guid>
    {
        public ServiceTalkFlyers()
        {
            ServiceTalkFlyerAttachments = new HashSet<ServiceTalkFlyerAttachments>();
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
        public int? ReadingTime { get; set; }
        public long ViewCount { get; set; }
        public bool IsH1 { get; set; }
        public bool IsH2 { get; set; }
        public bool IsH3 { get; set; }
        public bool IsTBSM { get; set; }

        public virtual ICollection<ServiceTalkFlyerAttachments> ServiceTalkFlyerAttachments { get; set; }
    }
}
