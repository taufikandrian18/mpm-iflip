using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services.Dto
{
    public class SendOTPDto
    {
        public string UserCategory { get; set; } //Internal, Siswa, Guru
        public string Username { get; set; }
        public string Handphone { get; set; }
    }

    public class ConfirmOTPDto
    {
        public string Code { get; set; }
        public string Key { get; set; }
    }

    public class ServiceResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class UploadResult : ServiceResult 
    {
        public Guid? Id { get; set; }
    }

    public class LoginExtensionDto
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public int IDJabatan { get; set; }
        public string IDGroupJabatan { get; set; }
        public string Jabatan { get; set; }
        public string Channel { get; set; }
        public string KodeDealer { get; set; }
        public string Dealer { get; set; }
        public string Kota { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAbleToRegisterCSChampionClub { get; set; }
    }
}
