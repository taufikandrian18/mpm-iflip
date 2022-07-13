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
    public interface IBASTAttachmentAppService : IApplicationService
    {
        BaseResponse GetAll([FromQuery] Pagination request); 
        BASTAttachment GetById(Guid id);
        void Create(BASTAttachmentCreateDto input);
        void Update(BASTAttachmentUpdateDto input);
        void SoftDelete(Guid id);
    }
}
