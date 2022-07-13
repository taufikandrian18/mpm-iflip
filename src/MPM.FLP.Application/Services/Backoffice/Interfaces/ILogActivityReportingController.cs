using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.Services.Dto;

namespace MPM.FLP.Services.Backoffice
{
    public interface ILogActivityReportingController : IApplicationService
    {
        ActionResult ExportExcelDetail(LogActivityReportingFilterDto request); 
        ActionResult ExportExcelSummary(LogActivityReportingFilterDto request);
    }
}