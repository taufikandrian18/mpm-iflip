using MPM.FLP.Authorization.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services.Dto
{
    public class InternalUserDto
    {
        public int IDMPM { get; set; }
        public long? AbpUserId { get; set; }
        public int? IDHonda { get; set; }
        public string Nama { get; set; }
        public string NoKTP { get; set; }
        public string Alamat { get; set; }
        public string Channel { get; set; }
        public string Handphone { get; set; }
        public string Gender { get; set; }
        public string AkunInstagram { get; set; }
        public string AkunFacebook { get; set; }
        public string AkunTwitter { get; set; }

        public string Jabatan { get; set; }
        public string NamaAtasan { get; set; }
        public string DealerName { get; set; }
        public string DealerKota { get; set; }
        public string KodeDealerMPM { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateInternalUserDto 
    {
        public int AbpUserId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public User LastModifierUser { get; set; }
    }

    public class RegisterInternalUserDto 
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
