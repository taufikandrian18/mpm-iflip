using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class TBSMUserSiswas : EntityBase
    {
        public Guid GUIDSekolah { get; set; }
        public long? AbpUserId { get; set; }
        public string KodeMD { get; set; }
        public string NamaMD { get; set; }
        public string NPSN { get; set; }
        public string NIS { get; set; }
        public string TBSMCategory { get; set; } = "Siswa";
        public string Nama { get; set; }
        public string JenisKelamin { get; set; }
        public string TempatLahir { get; set; }
        public DateTime TanggalLahir { get; set; }
        public string Agama { get; set; }
        public string NoTelp { get; set; }
        public string Alamat { get; set; }
        public string Provinsi { get; set; }
        public string Kota { get; set; }
        public string Email { get; set; }

        [JsonIgnore]
        public virtual Sekolahs Sekolah { get; set; }
    }
}
