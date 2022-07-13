using System;
using Abp.Domain.Entities;

namespace MPM.FLP.FLPDb
{
    public class AccesoriesCatalogs : Entity<Guid>
    {
        public override Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public bool IsPublished { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string FeaturedImageUrl { get; set; }
        public string AccessoriesCode { get; set; }
    }
}
