using Abp.Application.Services;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System.Collections.Generic;

namespace MPM.FLP.Services
{
    public interface ILogActivityReportingAppService : IApplicationService
    {
        List<LogActivityReportingDetailDto> ExportExcelDetail(LogActivityReportingFilterDto request);
        List<LogActivityReportingSummaryDto> ExportExcelSummary(LogActivityReportingFilterDto request);
    }
}
