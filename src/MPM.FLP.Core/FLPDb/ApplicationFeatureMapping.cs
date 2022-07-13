using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class ApplicationFeatureMapping : EntityBase
    {
        public Guid GUIDFeature { get; set; }
        public string EnumChannel { get;set;}
        public int Status { get;set;}
       
        [JsonIgnore]
        public virtual ApplicationFeature ApplicationFeature { get; set; }
    }
}
