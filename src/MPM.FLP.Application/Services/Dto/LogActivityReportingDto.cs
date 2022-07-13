using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Runtime.Validation;
using MPM.FLP.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MPM.FLP.Services.Dto
{
    public class LogActivityReportingFilterDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? UserId { get; set; }
        public string PageName { get; set; }
        public string LogAction { get; set; }
    }

    public class LogActivityReportingSummaryDto
    {
        public string PageName { get; set; }
        public string Action { get; set; }
        public int Total { get; set; }
    }

    public class LogActivityReportingDetailDto
    {
        public int IDMPM { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Action { get; set; }
        public string OldData { get; set; }
        public string ResultData { get; set; }
    }
}
