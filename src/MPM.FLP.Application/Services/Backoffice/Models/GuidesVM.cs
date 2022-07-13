using System;
namespace MPM.FLP.Services.Backoffice
{
    public class GuidesVM
    {
        public Guid Id { get; set; }
        public Guid? GuideCategoryId { get; set; }
        public bool IsPublished { get; set; }
        public int Order { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }
        public bool H1 { get; set; }
        public bool H2 { get; set; }
        public bool H3 { get; set; }
        public int? ReadingTime { get; set; }
        public long ViewCount { get; set; }
    }
}
