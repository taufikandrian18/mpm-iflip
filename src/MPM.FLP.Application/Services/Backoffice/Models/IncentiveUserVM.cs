using System;
using System.Collections.Generic;

namespace MPM.FLP.Services.Backoffice
{
    public class IncentiveUserVM
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<IncentiveKotasVM> ListKota { get; set; }
        public List<IncentiveJabatanVM> ListJabatan { get; set; }
    }
}
