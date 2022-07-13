using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MPM.FLP.FLPDb.Shared
{
    public class BaseEntity<TPrimaryKey> : Entity<TPrimaryKey>, ISoftDelete
    {
        [Required]
        public DateTime CreationTime { get; set; }
        [Required, MaxLength(256)]
        public string CreatorUsername { get; set; }
        public DateTime? LastModificationTime { get; set; }
        [MaxLength(256)]
        public string LastModifierUsername { get; set; }
        public DateTime? DeletionTime { get; set; }
        [MaxLength(256)]
        public string DeleterUsername { get; set; }
        [Required]
        public bool IsDeleted { get; set; } = false;

        public BaseEntity() {

            CreationTime = DateTime.Now;
        }
    }
}
