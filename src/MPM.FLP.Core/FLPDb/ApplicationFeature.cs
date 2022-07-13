using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MPM.FLP.FLPDb
{
    public class ApplicationFeature : EntityBase
    {
        public string IconUrl {get;set;}
        public string MenuName {get;set;}
        public virtual ICollection<ApplicationFeatureMapping> ApplicationFeatureMapping { get; set; }
    }
}
