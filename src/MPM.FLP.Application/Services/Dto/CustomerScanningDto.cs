using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services.Dto
{
    public class CustomerScanningOptionDto
    {
        public List<VariabelDto> JenisKelamin { get; set; }
        public List<VariabelDto> Usia { get; set; }
        public List<VariabelDto> Pekerjaan { get; set; }
        public List<VariabelDto> SES { get; set; }
        public List<VariabelDto> TipeSebelum { get; set; }
        public List<VariabelDto> JumlahMotor { get; set; }
        public List<VariabelDto> AlasanBeli { get; set; }
    }

    public class CustomerScanningSubmitDto 
    {
        public int jenisKelaminId { get; set; }
        public int usiaId { get; set; }
        public int pekerjaanId { get; set; }
        public int sesId { get; set; }
        public int tipeSebelumId { get; set; }
        public int jumlahMotorId { get; set; }
        public int alasanBeliId { get; set; }
    }

    public class CustomerScanningResultDto 
    {
        public string Merk { get; set; }
        public double Peluang { get; set; }
    }

    public class CustomerScanningCalculationDto 
    {
        public int TipeMotorId { get; set; }
        public string NamaMotor { get; set; }
        public double Koefisien { get; set; }
    }

    public class VariabelDto 
    {
        public int Id { get; set; }
        public string Nama { get; set; }
    }
}
