using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class ContentBankAssigneeProofs : EntityBase
    {
        public Guid GUIDContentBankAssignee { get; set; }
        public Guid GUIDContentBankPlatform { get; set; }
        public long GUIDEmployee { get; set; }
        public string Extension { get; set; }
        public string AttachmentURL {get;set;}
        public string RelatedLink { get; set; }
        public int ViewCount { get; set; }
        public int ShareCount { get; set; }
        public int LikeCount { get; set; }
        public DateTime UploadDate { get; set; }
        [JsonIgnore]
        public virtual ContentBankAssignees ContentBankAssignee { get; set; }
    }
}
