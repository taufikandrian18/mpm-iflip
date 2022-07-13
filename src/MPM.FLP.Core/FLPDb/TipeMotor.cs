using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class TipeMotor : Entity<int>
    {
        public TipeMotor()
        {
            KoefisienRegresi = new HashSet<KoefisienRegresi>();
        }

        public override int Id { get; set; }
        public string Nama { get; set; }

        public virtual ICollection<KoefisienRegresi> KoefisienRegresi { get; set; }
    }
}
