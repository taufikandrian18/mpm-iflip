using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class BASTs : EntityBase
    {
        public Guid GUIDCategory { get; set; }
        public string Name { get; set; }
        public bool IsTbsm { get; set; }
        public bool IsH1 { get; set; }
        public bool IsH2 { get; set; }
        public bool IsH3 { get; set; }
        public string KodeAHM { get; set; }
        public string KodeMPM { get; set; }
        public string KodeH3AHM { get; set; }
        public string KodeH3MPM { get; set; }
        public string RoadLetter { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        [JsonIgnore]
        public virtual ICollection<BASTDetails> BASTDetails { get; set; }
        public virtual ICollection<BASTAssignee> BASTAssignee { get; set; }
    }
}
