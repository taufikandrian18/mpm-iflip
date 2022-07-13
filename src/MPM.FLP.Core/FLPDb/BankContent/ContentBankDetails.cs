using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class ContentBankDetails : EntityBase
    {
        public Guid GUIDContentBank { get; set; }
        public string Name {get;set;}
        public int Orders {get;set;}
        public string Description {get;set;}
        public string Caption {get;set;}
        public string Extension {get;set;}
        public string AttachmentURL {get;set;}

        [JsonIgnore]
        public virtual ContentBanks ContentBanks { get; set; }
        public virtual ICollection<ContentBankAssignees> ContentBankAssignees { get; set; }
    }
}
