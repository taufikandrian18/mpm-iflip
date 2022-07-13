using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class LiveQuizAnswers : Entity<Guid>
    {
        public override Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public Guid LiveQuizHistoryId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }

        [JsonIgnore]
        public virtual LiveQuizHistories LiveQuizHistory { get; set; }
    }
}
