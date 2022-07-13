﻿using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MPM.FLP.FLPDb
{
    public partial class Articles: Entity<Guid>
    {
        public Articles()
        {
            ArticleAttachments = new HashSet<ArticleAttachments>();
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
        public long ViewCount { get; set; }
        public bool H1 { get; set; }
        public bool H2 { get; set; }
        public bool H3 { get; set; }
        public string Resource { get; set; }
        public int? ReadingTime { get; set; }

        public virtual ICollection<ArticleAttachments> ArticleAttachments { get; set; }
    }
}
