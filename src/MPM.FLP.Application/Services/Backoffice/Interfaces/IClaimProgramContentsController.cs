using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.Services.Dto;

namespace MPM.FLP.Services.Backoffice
{
    public interface IClaimProgramContentsController : IApplicationService
    {
        ActionResult ExportExcel(FilterGetClaimerDto request);

    }
}