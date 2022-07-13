using Abp.Application.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.Services.Dto;

namespace MPM.FLP.Services.Backoffice
{
    public interface ISalesTargetController : IApplicationService
    {
        ActionResult DownloadTemplateSalesTarget(DownloadSalesTargetTemplateDto input);
        Task<String> ImportExcelSalesTarget(int Month, int Year, string CreatorUsername, IEnumerable<IFormFile> files);
    }
}
