using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MPM.FLP.FLPDb
{
    public class Channels : Entity<Guid>
    {
        public override Guid Id { get; set; }
        [Required]
        [MaxLength(4)]
        public string Channel { get; set; }
        public string Descriptions { get; set; }

        public DateTime CreationTime { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string LastModifierUsername { get; set; }
        public string DeleterUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
    }
}
