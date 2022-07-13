using Abp.Domain.Entities;
using System;
using System.Collections.Generic;

namespace MPM.FLP.FLPDb
{
    public partial class UserDetails : Entity<Guid>
    {
        public override Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public int Points { get; set; }
    }
}
