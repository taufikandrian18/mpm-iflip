using Abp.AutoMapper;
using MPM.FLP.FLPDb;
using System;

namespace MPM.FLP.Services.Dto
{
    [AutoMapTo(typeof(TBSMUserGurus))]
    public class TBSMUserGurusCreateDto
    {
        public Guid GUIDSekolah { get; set; }
        public string KodeMD { get; set; }
        public string NamaMD { get; set; }
        public string NPSN { get; set; }
        public string NIP { get; set; }
        public string Nama { get; set; }
        public string JenisKelamin { get; set; }
        public string TempatLahir { get; set; }
        public string TanggalLahir { get; set; }
        public string Agama { get; set; }
        public string NoTelp { get; set; }
        public string Alamat { get; set; }
        public string Provinsi { get; set; }
        public string Kota { get; set; }
        public string Email { get; set; }
        public string CreatorUsername { get; set; }
    }

    [AutoMapTo(typeof(TBSMUserGurus))]
    public class TBSMUserGurusUpdateDto
    {
        public Guid Id { get; set; }
        public Guid GUIDSekolah { get; set; }
        public string KodeMD { get; set; }
        public string NamaMD { get; set; }
        public string NPSN { get; set; }
        public string NIP { get; set; }
        public string Nama { get; set; }
        public string JenisKelamin { get; set; }
        public string TempatLahir { get; set; }
        public string TanggalLahir { get; set; }
        public string Agama { get; set; }
        public string NoTelp { get; set; }
        public string Alamat { get; set; }
        public string Provinsi { get; set; }
        public string Kota { get; set; }
        public string Email { get; set; }
        public string LastModifierUsername { get; set; }
    }

    public class TBSMUserGurusDeleteDto
    {
        public Guid Id { get; set; }
        public string DeleterUsername { get; set; }
    }
}
