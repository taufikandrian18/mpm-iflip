using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public partial class SDMSMessageWeb : Entity<Guid>
    {
        public SDMSMessageWeb()
        {
            SDMSMessageDetail = new HashSet<SDMSMessageWebDetail>();
        }
        public override Guid Id { get; set; }
    
        public string Subject { get; set; }
        public string Body { get; set; }
        public string SenderUsername { get; set; }
        public string SenderId { get; set; }
        public DateTime? CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string DeleterUsername { get; set; }
        public virtual ICollection<SDMSMessageWebDetail> SDMSMessageDetail { get; set; }



    }
}
