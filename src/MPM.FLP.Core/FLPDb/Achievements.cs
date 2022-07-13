using Abp.Domain.Entities;
using System;
using System.Collections.Generic;

namespace MPM.FLP.FLPDb
{
    public partial class Achievements : Entity<Guid>
    {
        public Achievements()
        {
            UserAchievements = new HashSet<UserAchievements>();
        }

        public override Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }

        public virtual ICollection<UserAchievements> UserAchievements { get; set; }
    }
}
