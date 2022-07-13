using System;
namespace MPM.FLP.Services.Backoffice
{
    public class MotivationCardsVM
    {
        public Guid id { get; set; }
        public string Title { get; set; }
        public string Order { get; set; }
        public bool IsPublished { get; set; }
    }
}
