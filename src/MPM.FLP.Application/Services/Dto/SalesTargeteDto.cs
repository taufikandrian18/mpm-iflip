using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services.Dto
{
    public class DownloadSalesTargetTemplateDto
    {
        public int? Month { get; set; }
        public int? Year { get; set; }
        public DateTime? Periode1Start { get; set; }
        public DateTime? Periode1End { get; set; }
        public DateTime? Periode2Start { get; set; }
        public DateTime? Periode2End { get; set; }
        public DateTime? Periode3Start { get; set; }
        public DateTime? Periode3End { get; set; }
        public DateTime? Periode4Start { get; set; }
        public DateTime? Periode4End { get; set; }
        public DateTime? Periode5Start { get; set; }
        public DateTime? Periode5End { get; set; }
        public DateTime? Periode6Start { get; set; }
        public DateTime? Periode6End { get; set; }
    }
}
