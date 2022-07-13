using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class InboxMessages : Entity<Guid>
    {
        public InboxMessages()
        {
            InboxAttachments = new HashSet<InboxAttachments>();
            InboxRecipients = new HashSet<InboxRecipients>();
        }

        public override Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }

        public string Title { get; set; }
        public string Contents { get; set; }
        public string FeaturedImageUrl { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        public bool? IsTraining { get; set; }
        public bool? IsRolePlay { get; set; }
        public bool? IsSelfRecording { get; set; }
        public Guid? GUIDCategory { get; set; }
        public string Link { get; set; }

        public virtual ICollection<InboxAttachments> InboxAttachments { get; set; }
        public virtual ICollection<InboxRecipients> InboxRecipients { get; set; }
        public virtual InboxMessageCategories InboxMessageCategory { get; set; }
    }
}
