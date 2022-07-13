using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class BASTAssignee : EntityBase
    {
        public Guid BASTsId { get; set; }
        public long? GUIDEmployee { get; set; }
        public string Jabatan { get; set; }
        public string DealerName { get; set; }
        public string Channel { get; set; }
        public string KodeJaringan { get; set; }
        public string TipeJaringan { get; set; }
        public string Kota { get; set; }
        
        //[JsonIgnore]
        //public virtual ICollection<BASTes> BASTs { get; set; }
    }
}
