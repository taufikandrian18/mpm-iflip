using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IBASTReportAttachmentAppService : IApplicationService
    {
        BaseResponse GetAll([FromQuery] Pagination request);
        BASTReportAttachment GetById(Guid id);
        void Create(BASTReportAttachmentCreateDto input);
        void Update(BASTReportAttachmentUpdateDto input);
        void SoftDelete(Guid id);
    }
}
