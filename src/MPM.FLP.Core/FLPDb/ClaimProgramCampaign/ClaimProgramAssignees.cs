using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class ClaimProgramAssignees : EntityBase
    {
        public Guid GUIDClaimProgramContent { get; set; }
        public int IDJabatan {get;set;}
        public bool IsPassed { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public virtual ClaimProgramContents ClaimProgramContents { get; set; }
    }
}
