using Abp.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class ForumThreads : Entity<Guid>
    {
        public ForumThreads()
        {
            ForumPosts = new HashSet<ForumPosts>();
        }

        public override Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }

        public string Title { get; set; }
        public string Contents { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }

        public string KodeDealerMPM { get; set; }
        public string Channel { get; set; }


        //[JsonIgnore]
        //public Dealers Dealers { get; set; }
        public virtual ICollection<ForumPosts> ForumPosts { get; set; }
    }
}
