using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public partial class Kotas : Entity<int>
    {
        public Kotas()
        {
            Kecamatans = new HashSet<Kecamatans>();
        }

        [NotMapped]
        public override int Id { get { return CountyId; } }

        [Key]
        public int CountyId { get; set; }
        public string NamaKota { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }

        public virtual ICollection<Kecamatans> Kecamatans { get; set; }
    }
}
