using Abp.AutoMapper;
using JetBrains.Annotations;
using MPM.FLP.MPMWallet;
using System;
using System.ComponentModel.DataAnnotations;

namespace MPM.FLP.Services.Dto.MpmWallet
{
    [AutoMap(typeof(MpmWalletRegisterRequest))]
    public class MpmWalletRegisterRequestDto
    {
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public string Phone { get; set; }
        //public DateTime? DateOfBirth { get; set; }
        //public string PlaceOfBirth { get; set; }
        //public EnumMpmWalletCustomerIdentityTypeDto IdentityType { get; set; }
        //public string IdentityNo { get; set; }
        //public string Address { get; set; }
        //[MaxLength(1)]
        //[MinLength(1)]
        //public string Gender { get; set; }
        //public string Country { get; set; }
        //public string Job { get; set; }
        //public string City { get; set; }
        //public string ZipCode { get; set; }
        //public EnumMpmWalletCustomerEducationDto Education { get; set; }
        //public string CaptureId { get; set; }
    }

    public enum EnumMpmWalletCustomerIdentityTypeDto
    {
        KTP = 1,
        Passport = 2,
        SIM = 3
    }

    public enum EnumMpmWalletCustomerEducationDto
    {
        PrimarySchool = 1,
        MiddleSchool = 2,
        HighSchool = 3,
        AssociateDegree = 4,
        BachelorDegree = 5,
        MasterDegree = 6,
        Doctoral = 7
    }

    [AutoMap(typeof(MpmWalletRegisterResponse))]
    public class MpmWalletRegisterResponseDto : MpmWalletResponseDto
    {
        public string WalletId { get; set; }
        public MpmWalletCustomerRequestDto Customer { get; set; }
        public string CustomerType { get; set; }
    }

    [AutoMap(typeof(MpmWalletCustomerRequest))]
    public class MpmWalletCustomerRequestDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    [AutoMap(typeof(MpmWalletCustomerResponse))]
    public class MpmWalletCustomerResponseDto : MpmWalletCustomerRequestDto
    {
        public string IdentityType { get; set; }
        public string Education { get; set; }
    }

    [AutoMap(typeof(MpmWalletCustomerIdentityType))]
    public class MpmWalletCustomerIdentityTypeDto
    {
        public static int KTP { get; } = 1;
        public static int Passport { get; } = 2;
        public static int SIM { get; } = 3;
    }

    [AutoMap(typeof(MpmWalletCustomerEducation))]
    public class MpmWalletCustomerEducationDto
    {
        public static int PrimarySchool { get; } = 1;
        public static int MiddleSchool { get; } = 2;
        public static int HighSchool { get; } = 3;
        public static int AssociateDegree { get; } = 4;
        public static int BachelorDegree { get; } = 5;
        public static int MasterDegree { get; } = 6;
        public static int Doctoral { get; } = 7;
    }
}
