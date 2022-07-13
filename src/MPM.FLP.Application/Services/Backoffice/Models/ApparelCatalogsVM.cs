using System;
namespace MPM.FLP.Services.Backoffice
{
    public class ApparelCatalogsVM
    {
        public Guid Id { get; set; }
        public Guid? ApparelCategoryId { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string ApparelCode { get; set; }
        public bool IsPublished { get; set; }
    }
}
