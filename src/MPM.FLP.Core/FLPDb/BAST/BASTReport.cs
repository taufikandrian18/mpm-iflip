using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class BASTReport : EntityBase
    {
        public Guid GUIDBAST { get; set; }
        public Guid GUIDReporter { get; set; }
        public string NamaReporter { get; set; }
        public Guid GUIDPenerima { get; set; }
        public string NamaPenerima { get; set; }
        public Guid GUIDUpdater { get; set; }
        public string NamaUpdater { get; set; }
        public string KodeAHM { get; set; }
        public string KodeMPM { get; set; }
        public string Feedback { get; set; }
        public int JumlahDatang { get; set; }
        public int JumlahReject { get; set; }

        //[JsonIgnore]
        //public virtual ICollection<BASTes> BASTs { get; set; }
    }
}
