using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services.Dto
{
    public class CustomerEngagementOptionDto
    {
        public List<KotaDto> Kota { get; set; }
        public List<string> Agama { get; set; }
        public List<string> Pekerjaan { get; set; }
    }

    public class KotaDto 
    {
        public int CountyId { get; set; }
        public string Kota { get; set; }
        public List<String> Kecamatan { get; set; }
    }

    public class CustomerEngagementSubmitDto
    {
        public int CountyId { get; set; }
        public string Kecamatan { get; set; }
        public int BulanLahir { get; set; }
        public int FLPId { get; set; } 
        public string Agama { get; set; }
        public DateTime TanggalBeliMulai { get; set; }
        public DateTime TanggalBeliTerakhir { get; set; }
        public string Pekerjaan { get; set; }
    }

    public class CustomerEngagementDataDto
    {
        public int KDSLSPERSON { get; set; }
        public string NAMAKONSUMEN { get; set; }
        public string TGLLAHIR { get; set; }
        public string AGAMA { get; set; }
        public string HANDPHONE { get; set; }
        public string KECAMATAN { get; set; }
        public string KOTA { get; set; }
        public string TGLBELIMOTOR { get; set; }
        public string KDDEALER { get; set; }
        public string PEKERJAAN { get; set; }
        public string EMAIL { get; set; }
        public string UNIT { get; set; }
        public string KODEWARNA { get; set; }
        public string CATEGORY { get; set; }

    }
}
