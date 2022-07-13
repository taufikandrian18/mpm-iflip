using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class KoefisienRegresi : Entity<int>
    {
        public override int Id { get; set; }
        public int VariabelId { get; set; }
        public int TipeMotorId { get; set; }
        public double Nilai { get; set; }

        public virtual TipeMotor TipeMotor { get; set; }
        public virtual Variabel Variabel { get; set; }
    }
}
