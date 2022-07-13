using JetBrains.Annotations;
using MPM.FLP.FLPDb;
using MPM.FLP.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace MPM.FLP.MPMWallet
{
    public class MpmWalletRegisterRequest
    {
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }
        public int? IdentityType { get; set; }
        public string IdentityNo { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Country { get; set; }
        public string Job { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public int? Education { get; set; }
        public string CaptureId { get; set; }

        public MpmWalletRegisterRequest() { }
        public MpmWalletRegisterRequest(InternalUsers internalUser)
        {
            Name = internalUser.Nama;
            Email = internalUser.Email;
            Phone = internalUser.Handphone;
        }

        public MpmWalletRegisterRequest(string email, ExternalUsers externalUser)
        {
            Name = externalUser.Name;
            Email = email;
            Phone = externalUser.Handphone;
        }
    }

    public class MpmWalletRegisterResponse : MpmWalletResponse
    {
        public string ClientId { get; set; }
        public string WalletId { get; set; }
        public MpmWalletCustomerResponse Customer { get; set; }
    }

    public class NewMpmRegisterResponse : MpmWalletResponse
    {
        [JsonProperty("urlRegistration")]
        public Uri UrlRegistration { get; set; }

        [JsonProperty("clientId")]
        public long ClientId { get; set; }
    }

    public partial class ResponseMessage
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("en")]
        public string En { get; set; }
    }

    public class MpmWalletCustomerRequest
    {
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }
        public int IdentityType { get; set; }
        public string IdentityNo { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Country { get; set; }
        public string Job { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public int Education { get; set; }
        public string CaptureId { get; set; }
    }

    public class MpmWalletCustomerResponse : MpmWalletCustomerRequest
    {
        public new string IdentityType { get; set; }
        public new string Education { get; set; }
        public string Type { get; set; }
    }

    public class MpmWalletCustomerIdentityType
    {
        public static int KTP { get; } = 1;
        public static int Passport { get; } = 2;
        public static int SIM { get; } = 3;
    }

    public class MpmWalletCustomerEducation
    {
        public static int PrimarySchool { get; } = 1;
        public static int MiddleSchool { get; } = 2;
        public static int HighSchool { get; } = 3;
        public static int AssociateDegree { get; } = 4;
        public static int BachelorDegree { get; } = 5;
        public static int MasterDegree { get; } = 6;
        public static int Doctoral { get; } = 7;
    }

    public enum EnumMpmWalletCustomerIdentityType
    {
        KTP = 1,
        Passport = 2,
        SIM = 3
    }

    public enum EnumMpmWalletCustomerEducation
    {
        PrimarySchool = 1,
        MiddleSchool = 2,
        HighSchool = 3,
        AssociateDegree = 4,
        BachelorDegree = 5,
        MasterDegree = 6,
        Doctoral = 7
    }
}
