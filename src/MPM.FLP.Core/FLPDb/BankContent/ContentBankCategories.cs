using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class ContentBankCategories : EntityBase
    {
        public string Name { get; set; }
        public int Orders { get; set; }
        public string AttachmentUrl { get; set; }

        [JsonIgnore]
        public virtual ICollection<ContentBanks> ContentBanks { get; set; }
    }
}
