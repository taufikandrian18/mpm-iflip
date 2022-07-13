﻿using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class SalesPrograms : Entity<Guid>
    {
        public SalesPrograms()
        {
            SalesProgramAttachments = new HashSet<SalesProgramAttachments>();
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
        public bool H1 { get; set; }
        public bool H2 { get; set; }
        public bool H3 { get; set; }
        public int? ReadingTime { get; set; }
        public long ViewCount { get; set; }

        public virtual ICollection<SalesProgramAttachments> SalesProgramAttachments { get; set; }
    }
}
