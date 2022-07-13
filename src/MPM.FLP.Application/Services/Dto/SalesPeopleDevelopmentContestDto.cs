using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services.Dto
{
    public class SPDCLeaderBoardDto
    {
        public int IDMPM { get; set; }
        public string NamaFLP { get; set; }
        public double TotalPoint { get; set; }
        public List<SPDCLeaderBoardDetailDto> DetailPoint { get; set; }
    }

    public class SPDCLeaderBoardDetailDto
    {
        public string Name { get; set; }
        public double TotalPoint { get; set; }
        public double Weight { get; set; }
        public double Point { get; set; }
    }
}
