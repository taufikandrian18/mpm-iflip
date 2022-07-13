using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class HomeworkQuizAssignments : Entity<Guid>
    {
        public override Guid Id { get; set; }
        public Guid HomeworkQuizId { get; set; }
        public int IDMPM { get; set; }

        public virtual HomeworkQuizzes HomeworkQuiz { get; set; }
        public virtual InternalUsers InternalUser { get; set; }
    }
}
