using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class ClubCommunityCategories : Entity<Guid>
    {
        public ClubCommunityCategories()
        {
            ClubCommunities = new HashSet<ClubCommunities>();
        }

        public override Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public int? Order { get; set; }
        public bool IsPublished { get; set; }

        public virtual ICollection<ClubCommunities> ClubCommunities { get; set; }
    }
}
