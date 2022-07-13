using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class Events : Entity<Guid>
    {
        public Events()
        {
            EventAssignments = new HashSet<EventAssignments>();
        }

        public override Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }
        public string InformationProduct { get; set; }
        public decimal Budget { get; set; }
        public string Location { get; set; }
        public int TargetParticipant { get; set; }
        public int TargetProspectDb { get; set; }
        public int TargetSales { get; set; }
        public int TargetTestRide { get; set; }

        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public virtual ICollection<EventAssignments> EventAssignments { get; set; }
    }
}
