using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.Web.Models.FLPMPM
{
    public class IncentiveUserVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<IncentiveKotasVM> ListKota { get; set; }
        public List<IncentiveJabatanVM> ListJabatan { get; set; }
    }
}
