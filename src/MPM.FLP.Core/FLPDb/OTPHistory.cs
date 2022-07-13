using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class OTPHistory : Entity<Guid>
    {
        public override Guid Id { get; set; }
        public string Handphone { get; set; }
        public string OTPCode { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}
