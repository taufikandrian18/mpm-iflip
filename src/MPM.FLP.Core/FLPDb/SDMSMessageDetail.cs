using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MPM.FLP.FLPDb
{
    public class SDMSMessageDetail : Entity<Guid>
    {
        public override Guid Id { get; set; }
        public DateTime? CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string DeleterUsername { get; set; }
        public Guid? MessageId { get; set; }
        public string RecipientId { get; set; }
        public string RecipientUsername { get; set; }
        public int ReadStatus { get; set; }

        [JsonIgnore]
        public virtual SDMSMessage SDMSMessage { get; set; }
       // public virtual SDMSMessageWeb SDMSMessageWeb { get; set; }
    }
}
