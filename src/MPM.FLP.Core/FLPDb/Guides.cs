using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MPM.FLP.FLPDb
{
    public partial class Guides : Entity<Guid>
    {
        public Guides()
        {
            GuideAttachments = new HashSet<GuideAttachments>();
        }

        public override Guid Id { get; set; }
        public bool IsTechnicalGuide { get; set; }
        public Guid? GuideCategoryId { get; set; }
        public Guid? GuideTechnicalCategoryId { get; set; }
        public DateTime? CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public bool IsPublished { get; set; }
        public int Order { get; set; }
        
        public string Title { get; set; }
        public string Contents { get; set; }
        public string FeaturedImageUrl { get; set; }
        public bool H1 { get; set; }
        public bool H2 { get; set; }
        public bool H3 { get; set; }
        public string Resource { get; set; }
        public int? ReadingTime { get; set; }
        public long ViewCount { get; set; }

        [JsonIgnore]
        public virtual GuideCategories GuideCategory { get; set; }
        public virtual GuideTechnicalCategories GuideTechnicalCategory { get; set; }
        public virtual ICollection<GuideAttachments> GuideAttachments { get; set; }
    }
}
