using Abp.AutoMapper;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MPM.FLP.Services.Dto
{
    [AutoMap(typeof(ActivityLogs))]
    public class AddActivityLogDto
    {
        [Required, MaxLength(256)]
        public string ContentType { get; set; }
        [Required, MaxLength(256)]
        public string ActivityType { get; set; }
        [Required, MaxLength(256)]
        public string ContentId { get; set; }
        [Required, MaxLength(256)]
        public string ContentTitle { get; set; }
    }

    [AutoMap(typeof(ActivityLogs))]
    public class GetActivityLogDto
    {
        public string Username { get; set; }
        public string ContentType { get; set; }
        public string ContentTypeText { get { return ActivityLogs.GetContentTypeText(ContentType); } }
        public string ActivityType { get; set; }
        public string ContentId { get; set; }
        public string ContentTitle { get; set; }
        public DateTime Time { get; private set; }
    }

    [AutoMap(typeof(ActivityLogs))]
    public class GetActivityLogSummaryDto
    {
        public string ContentType { get; set; }
        public string ContentTypeText { get { return ActivityLogs.GetContentTypeText(ContentType); } }
        public string ActivityType { get; set; }
        public int Count { get; set; }
        public string Username { get; set; }
        public DateTime Time { get; set; }
    }

    [AutoMap(typeof(ActivityLogs))]
    public class GetContentActivityLogSummaryDto
    {
        public string ContentType { get; set; }
        public string ContentTypeText { get { return ActivityLogs.GetContentTypeText(ContentType); } }
        public string ActivityType { get; set; }
        public string ContentId { get; set; }
        public string ContentTitle { get; set; }
        public int Count { get; set; }
        public string Username { get; set; }
        public DateTime Time { get; set; }
    }

    public class GetSummaryActivityLog
    {
        public string Action { get; set; }
        public int Count { get; set; }
        public List<GetSummaryActivityLogDetail> Details { get; set; } = new List<GetSummaryActivityLogDetail>();
    }

    public class GetSummaryActivityLogDetail
    {
        public string PageName { get; set; }
        public int Count { get; set; }
    }
}
