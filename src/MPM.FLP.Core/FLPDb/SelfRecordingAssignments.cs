using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class SelfRecordingAssignments : Entity<Guid>
    {
        public override Guid Id { get; set; }
        public Guid SelfRecordingId { get; set; }
        public string KodeDealerMPM { get; set; }
        public string NamaDealer { get; set; }

        public virtual SelfRecordings SelfRecording { get; set; }
    }
}
