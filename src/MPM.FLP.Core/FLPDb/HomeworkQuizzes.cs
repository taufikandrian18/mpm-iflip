using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class HomeworkQuizzes : Entity<Guid>
    {
        public HomeworkQuizzes()
        {
            HomeworkQuizAssignments = new HashSet<HomeworkQuizAssignments>();
            HomeworkQuizHistories = new HashSet<HomeworkQuizHistories>();
            HomeworkQuizQuestions = new HashSet<HomeworkQuizQuestions>();
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
        public int Duration { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsPublished { get; set; }
        public bool? IsAlreadyTaken { get; set; }
        public decimal? MinimalScore { get; set; }
        public string FeaturedImageUrl { get; set; }

        [JsonIgnore]
        public virtual ICollection<HomeworkQuizAssignments> HomeworkQuizAssignments { get; set; }
        [JsonIgnore]
        public virtual ICollection<HomeworkQuizHistories> HomeworkQuizHistories { get; set; }
        public virtual ICollection<HomeworkQuizQuestions> HomeworkQuizQuestions { get; set; }
    }
}
