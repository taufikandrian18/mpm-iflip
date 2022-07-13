using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class Variabel : Entity<int>
    {
        public Variabel()
        {
            KoefisienRegresi = new HashSet<KoefisienRegresi>();
        }

        public override int Id { get; set; }
        public string Nama { get; set; }
        public int? JenisVariabelId { get; set; }

        public virtual JenisVariabel JenisVariabel { get; set; }
        public virtual ICollection<KoefisienRegresi> KoefisienRegresi { get; set; }
    }
}
