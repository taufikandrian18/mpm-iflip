using Abp.Domain.Entities;
using System;
using System.Collections.Generic;

namespace MPM.FLP.FLPDb
{
    public partial class UserAchievements : Entity<Guid>
    {
        public override Guid Id { get; set; }
        public string Username { get; set; }
        public Guid AchievementId { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }

        public virtual Achievements Achievement { get; set; }
    }
}
