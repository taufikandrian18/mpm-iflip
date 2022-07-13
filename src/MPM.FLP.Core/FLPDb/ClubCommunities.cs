using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class ClubCommunities : Entity<Guid>
    {
        
        public override Guid Id { get; set; }
        public Guid? ClubCommunityCategoryId { get; set; }
        public DateTime? CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }

        public string Name { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Kota { get; set; }

        [JsonIgnore]
        public virtual ClubCommunityCategories ClubCommunityCategories { get; set; }
    }
}
