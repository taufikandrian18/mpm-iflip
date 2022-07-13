using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class Sales : EntityBase
    {
        public string DealerId { get; set; }
        public DateTime TransactionDate { get; set; }
        public int Amount { get; set; }
        public Guid GUIDProductSeries { get; set; }
        public Guid GUIDProductType { get; set; }
        public Guid GUIDFincoy { get; set; }
        public string EnumPaymentCategory { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        [JsonIgnore]
        public virtual ProductTypes ProductTypes { get; set; }
        public virtual ProductSeries ProductSeries { get; set; }
        public virtual Fincoy Fincoy { get; set; }
    }
}
