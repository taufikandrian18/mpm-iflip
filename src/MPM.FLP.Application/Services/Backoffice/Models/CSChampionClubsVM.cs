using System;
namespace MPM.FLP.Services.Backoffice
{
    public class CSChampionClubsVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }
        public int? ReadingTime { get; set; }
        public long ViewCount { get; set; }
        public bool IsPublished { get; set; }
    }
}
