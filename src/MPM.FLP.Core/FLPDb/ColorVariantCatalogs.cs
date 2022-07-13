using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public partial class ColorVariantCatalogs : Entity<Guid>
    {
        public override Guid Id { get; set; }
        public string KodeType { get; set; }
        public string NamaType { get; set; }
        public string KodeWarna { get; set; }
        public string NamaPasar { get; set; }
        public string KategoriType { get; set; }
        public string NamaWarna { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
    }
}
