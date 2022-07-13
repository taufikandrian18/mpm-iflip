using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class RolePlays : Entity<Guid>
    {
        public RolePlays()
        {
            RolePlayDetails = new HashSet<RolePlayDetails>();
            RolePlayResults = new HashSet<RolePlayResults>();
            RolePlayAssignments = new HashSet<RolePlayAssignments>();
        }

        public override Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string Title { get; set; }
        public int? Order { get; set; }

        public virtual ICollection<RolePlayDetails> RolePlayDetails { get; set; }
        public virtual ICollection<RolePlayResults> RolePlayResults { get; set; }
        public virtual ICollection<RolePlayAssignments> RolePlayAssignments { get; set; }
    }
}
