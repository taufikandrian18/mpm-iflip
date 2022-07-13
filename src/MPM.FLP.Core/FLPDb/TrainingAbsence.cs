using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class TrainingAbsence : Entity<Guid>
    {
        public override Guid Id { get; set; }
        public int IDMPM { get; set; }
        public string IDTraining { get; set; }
        public DateTime FirstAbsence { get; set; }
        public DateTime? SecondAbsence { get; set; }
}
}
