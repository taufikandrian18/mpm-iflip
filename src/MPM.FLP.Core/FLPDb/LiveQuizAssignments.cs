using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class LiveQuizAssignments : Entity<Guid>
    {
        public override Guid Id { get; set; }
        public Guid LiveQuizId { get; set; }
        public int IDMPM { get; set; }

        public virtual LiveQuizzes LiveQuiz { get; set; }
        public virtual InternalUsers InternalUser { get; set; }
    }
}
