using Abp.Domain.Entities;
using System;

namespace MPM.FLP.FLPDb
{
    public class TBPermissions : Entity<Guid>
    {
        public override Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
