using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class Jabatans : Entity<int>
    {
        public Jabatans()
        {
            //InternalUsers = new HashSet<InternalUsers>();
            //VerifikasiJabatans = new HashSet<VerifikasiJabatans>();
        }


        [NotMapped]
        public override int Id { get { return IDJabatan; } }

        public string Channel { get; set; }
        [Key]
        public int IDJabatan { get; set; }
        public string Nama { get; set; }
        public string IDGroupJabatan { get; set; }
        public string NamaGroupJabatan { get; set; }

        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }


        //public virtual ICollection<InternalUsers> InternalUsers { get; set; }
        //public virtual ICollection<VerifikasiJabatans> VerifikasiJabatans { get; set; }
    }
}
