using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class BASTAttachment : EntityBase
    {
        public Guid BASTDetailsId { get; set; }
        public string Name { get; set; }
        public string AttachmentUrl { get; set; }
        public string FileName { get; set; }
        
        //[JsonIgnore]
        //public virtual ICollection<BASTDetails> BASTDetail { get; set; }
    }
}
