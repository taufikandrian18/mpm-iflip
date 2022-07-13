using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class Fincoy : EntityBase
    {
        public string FincoyName { get; set; }
        public virtual ICollection<Sales> Sales { get; set; }

    }
}
