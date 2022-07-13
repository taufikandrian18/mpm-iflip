using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class SalesIncentiveProgramJabatans : Entity<Guid>
    {
        public override Guid Id { get; set; }
        public Guid SalesIncentiveProgramId { get; set; }
        public string NamaJabatan { get; set; }

        [JsonIgnore]
        public virtual SalesIncentivePrograms SalesIncentiveProgram { get; set; }
    }
}
