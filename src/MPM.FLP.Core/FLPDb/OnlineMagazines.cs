using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class OnlineMagazines : Entity<Guid>
    {
        public override Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string Title { get; set; }
        public string CoverUrl { get; set; }
        public string StorageUrl { get; set; }
        public string Order { get; set; }
        public bool IsPublished { get; set; }

    }
}
