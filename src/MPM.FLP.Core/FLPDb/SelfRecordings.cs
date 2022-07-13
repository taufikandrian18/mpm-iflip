using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class SelfRecordings : Entity<Guid>
    {
        public SelfRecordings()
        {
            SelfRecordingDetails = new HashSet<SelfRecordingDetails>();
            SelfRecordingResults = new HashSet<SelfRecordingResults>();
            SelfRecordingAssignments = new HashSet<SelfRecordingAssignments>();
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

        public virtual ICollection<SelfRecordingDetails> SelfRecordingDetails { get; set; }
        public virtual ICollection<SelfRecordingResults> SelfRecordingResults { get; set; }
        public virtual ICollection<SelfRecordingAssignments> SelfRecordingAssignments { get; set; }
    }
}
