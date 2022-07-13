using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class InternalUsers : Entity<int>
    {

        public InternalUsers()
        {
            CSChampionClubParticipants = new HashSet<CSChampionClubParticipants>();
            HomeworkQuizAssignments = new HashSet<HomeworkQuizAssignments>();
            InboxRecipients = new HashSet<InboxRecipients>();
            EventAssignments = new HashSet<EventAssignments>();
            AgendaAssignments = new HashSet<AgendaAssignments>();
            SPDCPointHistories = new HashSet<SPDCPointHistories>();
        }

        [NotMapped]
        public override int Id { get { return IDMPM; } }

        [Key]
        public int IDMPM { get; set; }
        public long? AbpUserId { get; set; }
        public int? IDHonda { get; set; }
        public string Nama { get; set; }
        public string NoKTP { get; set; }
        public string Email { get; set; }
        public string Alamat  { get; set; }
        public string Channel { get; set; }
        public string Handphone   { get; set; }
        public string Gender { get; set; }
        public string AkunInstagram { get; set; }
        public string AkunFacebook { get; set; }
        public string AkunTwitter { get; set; }
        public int IDJabatan { get; set; }
        public string Jabatan { get; set; }
        public string IDGroupJabatan { get; set; }
        public int? MPMKodeAtasan { get; set; }
        public string NamaAtasan { get; set; }
        public string IDGroupJabatanAtasan { get; set; }
        public string JabatanAtasan { get; set; }
        public string KodeDealerAHM { get; set; }
        public string KodeDealerMPM { get; set; }
        public string DealerName { get; set; }
        public string DealerKota { get; set; }
        public string NamaFileFoto { get; set; }

        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }

        //[JsonIgnore]
        //public virtual Dealers Dealers { get; set; }

        //[JsonIgnore]
        //public virtual Jabatans Jabatans { get; set; }

        [JsonIgnore]
        public virtual ICollection<CSChampionClubParticipants> CSChampionClubParticipants { get; set; }
        [JsonIgnore]
        public virtual ICollection<HomeworkQuizAssignments> HomeworkQuizAssignments { get; set; }
        [JsonIgnore]
        public virtual ICollection<InboxRecipients> InboxRecipients { get; set; }
        [JsonIgnore]
        public virtual ICollection<EventAssignments> EventAssignments { get; set; }
        [JsonIgnore]
        public virtual ICollection<AgendaAssignments> AgendaAssignments { get; set; }
        [JsonIgnore]
        public virtual ICollection<SPDCPointHistories> SPDCPointHistories { get; set; }
    }
}
