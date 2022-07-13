using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.AutoMapper;
using MPM.FLP.FLPDb;

namespace MPM.FLP.Services.Dto {
    [AutoMap(typeof(Points))]
    public class AddPointDto
    {
        public ActivityLogs Activity { get; set; }
        public int Point { get; set; }
    }

    [AutoMap(typeof(Points))]
    public class PointDto {
        public ActivityLogs Activity {get;set;}
        public int Point {get;set;}
    }

    [AutoMap(typeof(PointConfigurations))]
    public class ContentPointConfigurationDto
    {
        public int Point { get; set; }
        public int Threshold { get; set; }
    }

    [AutoMap(typeof(PointConfigurations))]
    public class PointConfigurationDto
    {
        public Guid Id { get; set; }
        public string ContentType { get; set; }
        public string ActivityType { get; set; }
        public int Point { get; set; }
        public int DefaultThreshold { get; set; }
        public DateTime? EffDateFrom { get; set; }
        public DateTime? EffDateTo { get; set; }
        public bool IsDefault { get; set; }
    }

    [AutoMap(typeof(PointConfigurations))]
    public class AddPointConfigurationDto
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
        public bool IsDefault { get; set; }
    }

    [AutoMap(typeof(PointConfigurations))]
    public class UpdatePointConfigurationDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public int Point { get; set; }
        [Required]
        public int DefaultThreshold { get; set; }
        public DateTime? EffDateFrom { get; set; }
        public DateTime? EffDateTo { get; set; }
        public bool IsDefault { get; set; }
    }

    [AutoMap(typeof(PointConfigurations))]
    public class PointConfigurationActivityDto
    {
        [Required, MaxLength(256)]
        public string ContentType { get; set; }
        [Required, MaxLength(256)]
        public string ActivityType { get; set; }
    }

    [AutoMap(typeof(PointConfigurations))]
    public class PointConfigurationActivityContentDto
    {
        [Required, MaxLength(256)]
        public string ContentType { get; set; }
        [Required, MaxLength(256)]
        public string ActivityType { get; set; }
        [Required, MaxLength(256)]
        public string ContentId { get; set; }
    }
}