using MPM.FLP.Authorization.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services.Dto
{
    public class TbsmUserDto
    {
        public string NPSN { get; set; }
        public string TBSMCategory { get; set; }
        public string NomorInduk { get; set; }
    }

    public class RegisterTbsmUserDto
    {
        public string NPSN { get; set; }
        public string TBSMCategory { get; set; }
        public string NomorInduk { get; set; }
        public string Password { get; set; }
    }
}
