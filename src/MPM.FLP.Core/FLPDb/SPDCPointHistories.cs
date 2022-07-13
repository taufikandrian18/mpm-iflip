using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class SPDCPointHistories : Entity<Guid>
    {
        public override Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public int IDMPM { get; set; }
        public Guid SPDCMasterPointId { get; set; }
        public int Point { get; set; }
        public DateTime Periode { get; set; }

        public virtual InternalUsers InternalUsers { get; set; }
        public virtual SPDCMasterPoints SPDCMasterPoints { get; set; }

    }
}
