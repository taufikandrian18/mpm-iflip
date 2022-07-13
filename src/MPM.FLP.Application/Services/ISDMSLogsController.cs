using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using MPM.FLP.Services.Backoffice;

namespace MPM.FLP.Services
{
    public interface ISDMSLogsController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        SDMSLogs GetByIDBackoffice(Guid guid);
        SDMSLogs CreateBackoffice(SDMSLogs model);
        SDMSLogs UpdateBackoffice(SDMSLogs model);
        String DestroyBackoffice(Guid guid);
        IActionResult DownloadAll(Pagination request);
    }
}
