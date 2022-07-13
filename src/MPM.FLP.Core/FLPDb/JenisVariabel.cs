using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class JenisVariabel : Entity<int>
    {
        public JenisVariabel()
        {
            Variabel = new HashSet<Variabel>();
        }

        public override int Id { get; set; }
        public string Nama { get; set; }

        public virtual ICollection<Variabel> Variabel { get; set; }
    }
}
