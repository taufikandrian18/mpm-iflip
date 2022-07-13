using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class SalesIncentiveProgramTarget : EntityBase
    {
        public Guid SalesIncentiveProgramsId { get; set; }
        public string Kota { get; set; }
        public string DealerId { get; set; }
        public string DealerName { get; set; }
        public string Karesidenan { get; set; }
        public string EnumTipeTransaksi { get; set; }
        public int Target { get; set; }
        public int Transaksi { get; set; }
        public int Capaian { get; set; }

        //[JsonIgnore]
        //public virtual SalesIncentivePrograms SalesIncentivePrograms { get; set; }
    }
}
