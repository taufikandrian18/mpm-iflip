using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class ProductTypes : EntityBase
    {
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public virtual ICollection<ProductSeries> ProductSeries { get; set; }
        public virtual ICollection<Sales> Sales { get; set; }
        public virtual ICollection<SalesIncentivePrograms> SalesIncentivePrograms { get; set; }

    }
}
