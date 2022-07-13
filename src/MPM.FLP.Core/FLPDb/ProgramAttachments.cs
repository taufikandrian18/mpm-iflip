using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
namespace MPM.FLP.FLPDb
{
    public class ProgramAttachments : Entity<Guid>
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
        public Guid ProgramId { get; set; }
        public string Order { get; set; }

        [JsonIgnore]
        public virtual Programs Programs { get; set; }
    }
}
