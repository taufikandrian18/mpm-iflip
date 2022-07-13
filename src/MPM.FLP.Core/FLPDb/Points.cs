using Abp.Domain.Entities;
using MPM.FLP.FLPDb.Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace MPM.FLP.FLPDb
{
    public class PointConfigurations : BaseEntity<Guid>
    {
        [Required, MaxLength(256)]
        public string ContentType { get; set; }
        [Required, MaxLength(256)]
        public string ActivityType { get; set; }
        [Required]
        public int Point { get; set; }
        [Required]
        public int DefaultThreshold { get; set; }
        public DateTime? EffDateFrom { get; set; }
        public DateTime? EffDateTo { get; set; }
        [Required]
        public bool IsDefault { get; set; }

        public PointConfigurations() { }
    }

    public class Points : Entity<Guid>
    {
        [Required, MaxLength(256)]
        public string Username { get; set; }
        [Required]
        public Guid ActivityLogId { get; set; }
        public virtual ActivityLogs ActivityLog { get; set; }
        [Required]
        public int Point { get; set; }
        public DateTime ExpiryTime { get; private set; }

        public Points()
        {
            ExpiryTime = DateTime.Now.Date.AddYears(1).Date;
        }
    }
}
