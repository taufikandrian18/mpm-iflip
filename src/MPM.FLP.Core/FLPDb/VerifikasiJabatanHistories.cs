using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class VerifikasiJabatanHistories : Entity<Guid>
    {
        public override Guid Id { get; set; }
        public int IDMPM { get; set; }
        public bool? IsVerified { get; set; }
        public int? IDHonda { get; set; }
        public string Nama { get; set; }
        public string NoKTP { get; set; }
        public string Email { get; set; }
        public string Alamat { get; set; }
        public string Channel { get; set; }
        public string Handphone { get; set; }
        public string Gender { get; set; }
        public int? IDJabatan { get; set; }
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
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
    }
}
