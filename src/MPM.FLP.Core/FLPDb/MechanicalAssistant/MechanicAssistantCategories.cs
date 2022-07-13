using Newtonsoft.Json;
using System.Collections.Generic;

namespace MPM.FLP.FLPDb.MechanicalAssistant
{
    public class MechanicAssistantCategories : EntityBase
    {
        public string Name { get; set; }
 
        [JsonIgnore]
        public virtual ICollection<MechanicAssistants> MechanicAssistants { get; set; }
        public virtual ICollection<MechanicAssistantAssignees> MechanicAssistantAssignees { get; set; }
    }
}
