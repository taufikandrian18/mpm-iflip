using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class InboxRecipients : Entity<Guid>
    {
        public override Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public int IDMPM { get; set; }
        public Guid InboxMessageId { get; set; }
        public bool? IsRead { get; set; }

        [JsonIgnore]
        public virtual InboxMessages InboxMessages { get; set; }
        [JsonIgnore]
        public virtual InternalUsers InternalUser { get; set; }
    }
}
