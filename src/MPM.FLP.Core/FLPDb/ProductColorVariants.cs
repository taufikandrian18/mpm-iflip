using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MPM.FLP.FLPDb
{
    public class ProductColorVariants : Entity<Guid>
    {
        public ProductColorVariants()
        {
            ProductPrices = new HashSet<ProductPrices>();
        }

        public override Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public Guid ProductCatalogId { get; set; }
        public string HexColor { get; set; }
        public decimal? Price { get; set; }
        public string ColorCode { get; set; }

        [JsonIgnore]
        public virtual ProductCatalogs ProductCatalogs { get; set; }

        [JsonIgnore]
        public virtual ICollection<ProductPrices> ProductPrices { get; set; }
    }
}
