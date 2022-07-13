using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class TargetSales : EntityBase
    {
        public string DealerId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime? Periode1Start { get; set; }
        public DateTime? Periode1End { get; set; }
        public int Periode1Target { get; set; }
        public DateTime? Periode2Start { get; set; }
        public DateTime? Periode2End { get; set; }
        public int Periode2Target { get; set; }
        public DateTime? Periode3Start { get; set; }
        public DateTime? Periode3End { get; set; }
        public int Periode3Target { get; set; }
        public DateTime? Periode4Start { get; set; }
        public DateTime? Periode4End { get; set; }
        public int Periode4Target { get; set; }
        public DateTime? Periode5Start { get; set; }
        public DateTime? Periode5End { get; set; }
        public int Periode5Target { get; set; }
        public DateTime? Periode6Start { get; set; }
        public DateTime? Periode6End { get; set; }
        public int Periode6Target { get; set; }
        public int TargetTotal { get; set; }
        public int Achievement { get; set; }
    }
}
