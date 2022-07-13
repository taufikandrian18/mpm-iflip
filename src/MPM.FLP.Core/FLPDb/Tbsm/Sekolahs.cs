using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class Sekolahs : EntityBase
    {
        public string KodeMD { get; set; }
        public string NamaMD { get; set; }
        public string NPSN { get; set; }
        public string NamaSMK { get; set; }
        public string StatusSMK { get; set; }
        public string Alamat { get; set; }
        public string Provinsi { get; set; }
        public string Kota { get; set; }
        public string NoTelp { get; set; }
        public string NoFax { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Grade { get; set; }
        public bool StatusTSMHonda { get; set; }
        public string AkreditasiProdiTSM { get; set; }
        public bool StatusTUKHonda { get; set; }

        [JsonIgnore]
        public virtual ICollection<TBSMUserGurus> TBSMUserGurus { get; set; }
        public virtual ICollection<TBSMUserSiswas> TBSMUserSiswas { get; set; }
    }
}
