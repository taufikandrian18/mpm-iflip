using System;
namespace MPM.FLP.Services.Backoffice
{
    public class ApparelCategoriesVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public int? Order { get; set; }
        public bool IsPublished { get; set; }
    }
}
