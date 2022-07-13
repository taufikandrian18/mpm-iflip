using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class ProductPrices : Entity<Guid>
    {
        public override Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string KodeDealerMPM { get; set; }
        public decimal Price { get; set; }
        public Guid ProductColorVariantId { get; set; }

        [JsonIgnore]
        public virtual ProductColorVariants ProductColorVariants { get; set; }
        //[JsonIgnore]
        //public virtual Dealers Dealers { get; set; }
    }
}
