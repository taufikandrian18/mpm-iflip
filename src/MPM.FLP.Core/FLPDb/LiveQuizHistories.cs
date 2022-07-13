using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class LiveQuizHistories : Entity<Guid>
    {
        public LiveQuizHistories()
        {
            LiveQuizAnswers = new HashSet<LiveQuizAnswers>();
        }

        public override Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public Guid LiveQuizId { get; set; }
        public int? IDMPM { get; set; }
        public string Name { get; set; }
        public string Jabatan { get; set; }
        public string Kota { get; set; }
        public string Dealer { get; set; }
        public int? CorrectAnswer { get; set; }
        public int? WrongAnswer { get; set; }
        public decimal? Score { get; set; }

        [JsonIgnore]
        public virtual LiveQuizzes LiveQuiz { get; set; }
        public virtual ICollection<LiveQuizAnswers> LiveQuizAnswers { get; set; }
    }
}
