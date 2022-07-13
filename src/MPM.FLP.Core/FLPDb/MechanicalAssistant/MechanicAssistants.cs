using Newtonsoft.Json;
using System;

namespace MPM.FLP.FLPDb.MechanicalAssistant
{
    public class MechanicAssistants : EntityBase
    {
        public Guid GUIDCategory { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public string MIME { get; set; }
        public string Attachment { get; set; }
        public bool IsH1 { get; set; }
        public bool IsH2 { get; set; }
        public bool IsH3 { get; set; }
        public bool IsTBSM { get; set; }

        [JsonIgnore]
        public virtual MechanicAssistantCategories MechanicAssistantCategory { get; set; }
    }
}
