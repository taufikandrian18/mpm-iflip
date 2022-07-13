using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class ClaimProgramContentAttachments : EntityBase
    {
        public Guid GUIDClaimProgramContent { get; set; }
        public string Name {get;set;}
        public string  Category { get; set; }
        public string AttachmentURL {get;set;}
        [JsonIgnore]
        public virtual ClaimProgramContents ClaimProgramContents { get; set; }
    }
}
