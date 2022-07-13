using Newtonsoft.Json;
using System;

namespace MPM.FLP.FLPDb.MechanicalAssistant
{
    public class MechanicAssistantAssignees : EntityBase
    {
        public Guid GUIDCategory { get; set; }
        public Guid GUIDEmployee { get; set; }
        public string Jabatan { get; set; }
        public string DealerName { get; set; }
        public string Channel { get; set; }
        public string Kota { get; set; }

        [JsonIgnore]
        public virtual MechanicAssistantCategories MechanicAssistantCategory { get; set; }
    }
}
