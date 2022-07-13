using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class CSChampionClubs : Entity<Guid>
    {
        public CSChampionClubs()
        {
            CSChampionClubAttachments = new HashSet<CSChampionClubAttachments>();
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
        public bool? IsRegistered { get; set; }
        public bool? IsRegistrationCanceled { get; set; }
        public int? ReadingTime { get; set; }
        public long ViewCount { get; set; }

        public virtual ICollection<CSChampionClubAttachments> CSChampionClubAttachments { get; set; }
    }
}
