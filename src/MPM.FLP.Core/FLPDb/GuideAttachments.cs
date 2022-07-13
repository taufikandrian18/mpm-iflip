using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MPM.FLP.FLPDb
{
    public partial class GuideAttachments : Entity<Guid>
    {
        public override Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string Title { get; set; }
        public string StorageUrl { get; set; }
        public Guid GuideId { get; set; }
        public string Order { get; set; }
        public string FileName { get; set; }

        [JsonIgnore]
        public virtual Guides Guide { get; set; }
    }
}
