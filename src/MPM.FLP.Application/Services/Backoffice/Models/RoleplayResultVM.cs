using System;
namespace MPM.FLP.Services.Backoffice
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
