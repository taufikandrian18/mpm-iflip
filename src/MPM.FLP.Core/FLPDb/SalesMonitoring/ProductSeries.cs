using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class ProductSeries : EntityBase
    {
        public Guid GUIDProductType { get; set; }
        public string SeriesName { get; set; }
        public string SeriesCode { get; set; }
        public virtual ICollection<Sales> Sales { get; set; }
        [JsonIgnore]
        public virtual ProductTypes ProductTypes { get; set; }
        
    }
}
