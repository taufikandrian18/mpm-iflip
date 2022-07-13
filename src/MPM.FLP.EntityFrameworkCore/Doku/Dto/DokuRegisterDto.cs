using Abp.AutoMapper;
using MPM.FLP.MPMWallet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MPM.FLP.Doku.Dto
{
    public class DokuRegisterRequestDto
    {
        public DokuRegisterRequestDto(DokuSettings dokuSettings, MpmWalletRegisterRequest mpmWalletRegister)
        {
            ClientId = dokuSettings.ClientId;
            AccessToken = DokuSettings.AccessToken;
            Words = dokuSettings.GetHash(dokuSettings.ClientId + dokuSettings.SharedKey + mpmWalletRegister.Email + DokuSettings.Systrace);

            CustomerName = mpmWalletRegister.Name;
            CustomerEmail = mpmWalletRegister.Email;
            CustomerPhone = mpmWalletRegister.Phone;
            CustomerDoB = mpmWalletRegister.DateOfBirth?.ToString("yyyy-MM-dd");
            CustomerPoB = mpmWalletRegister.PlaceOfBirth;
            CustomerIdentityType = mpmWalletRegister.IdentityType;
            CustomerIdentityNo = mpmWalletRegister.IdentityNo;
            CustomerAddress = mpmWalletRegister.Address;
            CustomerGender = mpmWalletRegister.Gender;
            CustomerCountry = mpmWalletRegister.Country;
            CustomerJob = mpmWalletRegister.Job;
            CustomerCity = mpmWalletRegister.City;
            CustomerZipCode = mpmWalletRegister.ZipCode;
            CustomerEducation = mpmWalletRegister.Education;
            CustomerCaptureId = mpmWalletRegister.CaptureId;
        }

        public DokuRegisterRequestDto(DokuSettings dokuSettings, int idMpm)
        {
            ClientId = dokuSettings.ClientId;
            AccessToken = DokuSettings.AccessToken;
            Systrace = DokuSettings.Systrace;
            Words = dokuSettings.GetHash(dokuSettings.ClientId + DokuSettings.AccessToken  + DokuSettings.Systrace + dokuSettings.SharedKey);
            Version = "3.0";
            UrlIntent = "http://mpm-flp-api.azurewebsites.net/api/services/app/MpmWallet/Callback?IdMpm="+idMpm+"&AccountId=";
        }



        public string ClientId { get; set; }
        public string AccessToken { get; set; }
        public string Words { get; set; }
        public string Version { get; set; } = "3.0";
        public string Systrace { get; set; }
        public string UrlIntent { get; set; }

        public string CustomerName { get; set; }
        [Required]
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerDoB { get; set; }
        public string CustomerPoB { get; set; }
        public int? CustomerIdentityType { get; set; }
        public string CustomerIdentityNo { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerGender { get; set; }
        public string CustomerCountry { get; set; }
        public string CustomerJob { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerZipCode { get; set; }
        public int? CustomerEducation { get; set; }
        public string CustomerCaptureId { get; set; }
    }

    [AutoMapTo(typeof(MpmWalletRegisterResponse))]
    public class DokuRegisterResponseDto : MpmWalletRegisterResponse
    {
        [JsonProperty("DokuId")]
        public new string WalletId { get; set; }
    }
}
