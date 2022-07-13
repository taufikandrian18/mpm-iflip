using Abp.AutoMapper;
using MPM.FLP.FLPDb;
using System;

namespace MPM.FLP.Services.Dto
{
    [AutoMapTo(typeof(TBSMUserSiswas))]
    public class TBSMUserSiswasCreateDto
    {
        public Guid GUIDSekolah { get; set; }
        public string KodeMD { get; set; }
        public string NamaMD { get; set; }
        public string NPSN { get; set; }
        public string NIS { get; set; }
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

    [AutoMapTo(typeof(TBSMUserSiswas))]
    public class TBSMUserSiswasUpdateDto
    {
        public Guid Id { get; set; }
        public Guid GUIDSekolah { get; set; }
        public string KodeMD { get; set; }
        public string NamaMD { get; set; }
        public string NPSN { get; set; }
        public string NIS { get; set; }
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

    public class TBSMUserSiswasDeleteDto
    {
        public Guid Id { get; set; }
        public string DeleterUsername { get; set; }
    }
}
