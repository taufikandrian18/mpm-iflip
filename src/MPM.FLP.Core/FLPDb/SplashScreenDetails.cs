using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class SplashScreenDetails : EntityBase
    {
        public Guid GUIDSplashScreen { get; set; }
        public string Name { get; set; }
        public int Orders { get; set; }
        public string Caption { get; set; }
        public string Extension { get; set; }
        public string ImageUrl { get; set; }
        public string DeepLink { get; set; }

        [JsonIgnore]
        public virtual SplashScreen SplashScreen { get; set; }
    }
}
