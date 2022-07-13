using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.FLPDb
{
    public class AdminUsers : Entity<Guid>
    {
        public override Guid Id { get; set; }
        public long AbpUserId { get; set; }
        public string Channel { get; set; }

        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
    }
}
