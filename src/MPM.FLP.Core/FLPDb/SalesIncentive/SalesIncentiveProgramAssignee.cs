using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class SalesIncentiveProgramAssignee : EntityBase
    {
        public Guid SalesIncentiveProgramId { get; set; }
        public string DealerId { get; set; }
        public string DealerName { get; set; }
        public string Kota { get; set; }
        public string Jabatan { get; set; }

        [JsonIgnore]
        public virtual SalesIncentivePrograms SalesIncentivePrograms { get; set; }

    }
}
