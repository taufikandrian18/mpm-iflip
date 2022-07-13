using Abp.AutoMapper;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MPM.FLP.Services.Dto
{
    public class DealerH3SyncResponseDto
    {
        public int status { get; set; }
        public string message { get; set; }
        public List<DealerH3DetailSyncResponseDto> data { get; set; }
    }

    [AutoMapTo(typeof(DealerH3))]
    public class DealerH3DetailSyncResponseDto
    {
        public string accountnum { get; set; }
        public string ahmcode { get; set; }
        public string mdcode { get; set; }
        public string namadealer { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string channeldealer { get; set; }
        public string dlrEmail { get; set; }
        public string kodekareswil { get; set; }
        public string karesidenan { get; set; }
        public string npksupervisor { get; set; }
        public string namasupervisor { get; set; }
        public string email { get; set; }
        public string idkaresidenanhc3 { get; set; }
        public string namakareshc3 { get; set; }
        public string npkspvhc3 { get; set; }
        public string namaspvhc3 { get; set; }
        public string npkdeptheadhc3 { get; set; }
        public string namadeptheadhc3 { get; set; }
        public string npkdivheadhc3 { get; set; }
        public string namadivheadhc3 { get; set; }
        public string idkaresidenantsd { get; set; }
        public string namakarestsd { get; set; }
        public string npkspvtsd { get; set; }
        public string namaspvtsd { get; set; }
        public string npkdeptheadtsd { get; set; }
        public string namadeptheadtsd { get; set; }
        public string npkdivheadtsd { get; set; }
        public string namadivheadtsd { get; set; }
    }
}
