using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.Web.Models.FLPMPM
{
    public class PointConfigurationVM
    {
        public Guid Id { get; set; }
        public string ContentType { get; set; }
        public string ContentTypeText { get; set; }
        public string ActivityType { get; set; }
        public int DefaultPoint { get; set; }
        public int? ActivePoint { get; set; }
        public int DefaultThreshold { get; set; }
        public DateTime? EffDateFrom { get; set; }
        public DateTime? EffDateTo { get; set; }
        public bool IsDefault { get; set; }
    }
}
