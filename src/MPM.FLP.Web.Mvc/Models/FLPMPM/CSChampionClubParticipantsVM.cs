using System;

namespace MPM.FLP.Web.Models.FLPMPM
{
    public class CSChampionClubParticipantsVM
    {
        public Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public int IDMPM { get; set; }
        public string Name { get; set; }
        public string DealerName { get; set; }
        public Guid CSChampionClubId { get; set; }

    }
}
