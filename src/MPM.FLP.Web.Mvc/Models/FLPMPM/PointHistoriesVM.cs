using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.Web.Models.FLPMPM
{
    public class PointHistoriesVM
    {
        public Guid Id { get; set; }
        public string Nama { get; set; }
        public string Channel { get; set; }
        public string Dealer { get; set; }
        public string MasterPoint { get; set; }
        public int Point { get; set; }
        public DateTime Periode { get; set; }
    }
}
