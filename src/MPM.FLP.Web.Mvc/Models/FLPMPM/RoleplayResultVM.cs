using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.Web.Models.FLPMPM
{
    public class RoleplayResultVM
    {
        public Guid id { get; set; }
        public int idmpm { get; set; }
        public string namaFLP { get; set; }
        public string kodeDealerMPM { get; set; }
        public string namaDealerMPM { get; set; }
        public decimal? verificationResult { get; set; }
        public decimal? flpResult { get; set; }
        public string url { get; set; }
        public bool? isVerified { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
