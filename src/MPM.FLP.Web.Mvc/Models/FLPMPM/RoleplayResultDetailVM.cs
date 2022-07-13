using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.Web.Models.FLPMPM
{
    public class RoleplayResultDetailVM
    {
        public Guid id { get; set; }
        public string title { get; set; }
        public int? order { get; set; }
        public bool? isMandatorySilver { get; set; }
        public bool? isMandatoryGold { get; set; }
        public bool? isMandatoryPlatinum { get; set; }
        public bool? beforePassed { get; set; }
        public bool? beforeNotPassed { get; set; }
        public bool? beforeDismiss { get; set; }
        public bool? afterPassed { get; set; }
        public bool? afterNotPassed { get; set; }
        public bool? afterDismiss { get; set; }
        public Guid rolePlayId { get; set; }
        public Guid rolePlayResultId { get; set; }
    }
}
