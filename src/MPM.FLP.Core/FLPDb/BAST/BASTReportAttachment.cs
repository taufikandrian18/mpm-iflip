using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class BASTReportAttachment : EntityBase
    {
        public Guid GUIDBAST { get; set; }
        public Guid GUIDReport { get; set; }
        public string MIME { get; set; }
        public string AttachmentUrl { get; set; }
        public string FileName { get; set; }
        
        //[JsonIgnore]
        //public virtual ICollection<BASTes> BASTs { get; set; }
        //public virtual ICollection<BASTReport> BASTReport { get; set; }
    }
}
