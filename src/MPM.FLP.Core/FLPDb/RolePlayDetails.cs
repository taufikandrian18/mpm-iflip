using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class RolePlayDetails : Entity<Guid>
    {
        public override Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string Title { get; set; }
        public int? Order { get; set; }
        public bool IsMandatorySilver { get; set; }
        public bool IsMandatoryGold { get; set; }
        public bool IsMandatoryPlatinum { get; set; }
        public Guid RolePlayId { get; set; }

        [JsonIgnore]
        public virtual RolePlays RolePlay { get; set; }
    }
}
