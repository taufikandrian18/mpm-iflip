using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class ContentBanks : EntityBase
    {
        public Guid GUIDContentBankCategory { get; set; }
        public string Name {get;set;}
        public string Description {get;set;}
        public string Caption { get; set; }
        public int ReadingTime { get; set; }
        public bool H1 { get; set; }
        public bool H2 { get; set; }
        public bool H3 { get; set; }
        public DateTime? StartDate {get;set;}
        public DateTime? EndDate {get;set;}
        public bool IsPublished { get; set; }

        [JsonIgnore]
        public virtual ContentBankCategories ContentBankCategories { get; set; }
        public virtual ICollection<ContentBankDetails> ContentBankDetails { get; set; }
    }
}
