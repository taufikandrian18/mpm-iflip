using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class BASTDetails : EntityBase
    {
        public Guid BASTsId { get; set; }
        public string Name { get; set; }
        public int Qty { get; set; }

        [JsonIgnore]
        public virtual BASTs BASTs { get; set; }
        public virtual ICollection<BASTAttachment> BASTAttachment { get; set; }
    }
}
