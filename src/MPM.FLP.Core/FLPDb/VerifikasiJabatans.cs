using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class VerifikasiJabatans : Entity<Guid>
    {
        public VerifikasiJabatans()
        {
            VerifikasiJabatanQuestions = new HashSet<VerifikasiJabatanQuestions>();
        }

        public override Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string Title { get; set; }
        public string IDGroupJabatan { get; set; }
        public decimal PassingScore { get; set; }

        //[JsonIgnore]
        //public virtual Jabatans Jabatans { get; set; }
        public virtual ICollection<VerifikasiJabatanQuestions> VerifikasiJabatanQuestions { get; set; }
    }
}
