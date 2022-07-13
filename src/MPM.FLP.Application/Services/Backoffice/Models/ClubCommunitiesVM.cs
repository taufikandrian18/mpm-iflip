using System;
namespace MPM.FLP.Services.Backoffice
{
    public class ClubCommunitiesVM
    {
        public Guid ClubCommunityCategoryId { get; set; }
        public string Name { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public string Kota { get; set; }
    }
}
