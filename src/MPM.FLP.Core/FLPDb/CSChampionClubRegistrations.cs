using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class CSChampionClubRegistrations : Entity<int>
    {

        public CSChampionClubRegistrations()
        {
            CSChampionClubParticipants = new HashSet<CSChampionClubParticipants>(); 
        }

        [NotMapped]
        public override int Id { get { return Year; } }

        [Key]
        public int Year { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [JsonIgnore]
        public virtual ICollection<CSChampionClubParticipants> CSChampionClubParticipants { get; set; }
    }
}
