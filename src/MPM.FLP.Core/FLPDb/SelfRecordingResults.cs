using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class SelfRecordingResults : Entity<Guid>
    {
        public SelfRecordingResults()
        {
            SelfRecordingResultDetails = new HashSet<SelfRecordingResultDetails>();
        }

        public override Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public int IDMPM { get; set; }
        public string NamaFLP { get; set; }
        public string KodeDealerMPM { get; set; }
        public string NamaDealerMPM { get; set; }
        public decimal? FLPResult { get; set; }
        public decimal? VerificationResult { get; set; }
        public string FLPGrade { get; set; }
        public string VerificationGrade { get; set; }
        public bool? IsVerified { get; set; }
        public string StorageUrl { get; set; }
        public string YoutubeUrl { get; set; }
        public Guid SelfRecordingId { get; set; }

        [JsonIgnore]
        public virtual SelfRecordings SelfRecording { get; set; }
        public virtual ICollection<SelfRecordingResultDetails> SelfRecordingResultDetails { get; set; }
    }
}
