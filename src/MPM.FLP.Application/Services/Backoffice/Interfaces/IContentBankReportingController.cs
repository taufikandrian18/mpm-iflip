using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MPM.FLP.Services.Backoffice
{
    public interface IContentBankReportingController : IApplicationService
    {
        ActionResult ExportExcelDownload(string channel, string search); 
        ActionResult ExportExcelSosmed(string channel, string search);
    }
}