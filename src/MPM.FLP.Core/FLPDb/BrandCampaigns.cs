using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class BrandCampaigns : Entity<Guid>
    {
        public BrandCampaigns()
        {
            BrandCampaignAttachments = new HashSet<BrandCampaignAttachments>();
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
        public int? ReadingTime { get; set; }
        public long ViewCount { get; set; }

        public virtual ICollection<BrandCampaignAttachments> BrandCampaignAttachments { get; set; }
    }
}
