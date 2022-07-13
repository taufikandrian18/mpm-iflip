using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class LiveQuizzes : Entity<Guid>
    {
        public LiveQuizzes()
        {
            LiveQuizAssignments = new HashSet<LiveQuizAssignments>();
            LiveQuizHistories = new HashSet<LiveQuizHistories>();
            LiveQuizQuestions = new HashSet<LiveQuizQuestions>();
        }

        public override Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string Title { get; set; }
        public int TotalQuestion { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsPublished { get; set; }
        public string FeaturedImageUrl { get; set; }
        public bool? IsAlreadyTaken { get; set; }

        [JsonIgnore]
        public virtual ICollection<LiveQuizAssignments> LiveQuizAssignments { get; set; }
        [JsonIgnore]
        public virtual ICollection<LiveQuizHistories> LiveQuizHistories { get; set; }
        public virtual ICollection<LiveQuizQuestions> LiveQuizQuestions { get; set; }
    }
}
