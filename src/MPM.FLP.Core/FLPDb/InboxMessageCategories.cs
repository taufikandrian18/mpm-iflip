using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class InboxMessageCategories : EntityBase
    {

        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<InboxMessages> InboxMessages { get; set; }
    }
}
