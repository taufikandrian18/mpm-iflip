using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class SPDCMasterPoints : Entity<Guid>
    {
        public SPDCMasterPoints()
        {
            SPDCPointHistories = new HashSet<SPDCPointHistories>();
        }

        public override Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string Title { get; set; }
        public decimal Weight { get; set; }

        [JsonIgnore]
        public virtual ICollection<SPDCPointHistories> SPDCPointHistories { get; set; }
    }
}
