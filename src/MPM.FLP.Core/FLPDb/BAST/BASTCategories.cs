using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class BASTCategories : EntityBase
    {
        public string Name { get; set; }

        //[JsonIgnore]
        //public virtual ICollection<BASTContent> BASTContent { get; set; }
    }
}
