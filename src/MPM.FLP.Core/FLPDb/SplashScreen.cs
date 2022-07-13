using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MPM.FLP.FLPDb
{
    public class SplashScreen : EntityBase
    {
        public string Title {get;set;}
        public string Description {get;set;}
        public string Link { get; set; }
        public int Duration { get; set; }
        public bool H1 { get; set; }
        public bool H2 { get; set; }
        public bool H3 { get; set; }
        public bool IsTbsm { get; set; }
        public bool IsPublished { get; set; }

        public virtual ICollection<SplashScreenDetails> SplashScreenDetails { get; set; }
    }
}
