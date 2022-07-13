using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class Agendas : Entity<Guid>
    {
        public Agendas()
        {
            AgendaAssignments = new HashSet<AgendaAssignments>();
        }

        public override Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime ComingDate { get; set; }
        public string StorageUrl { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }

        public virtual ICollection<AgendaAssignments> AgendaAssignments { get; set; }
    }
}
