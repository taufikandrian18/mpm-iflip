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
    public interface IBASTAssigneeAppService : IApplicationService
    {
        BaseResponse GetAll([FromQuery] Pagination request); 
        BASTAssignee GetById(Guid id);
        void Create(BASTAssigneeCreateDto input);
        void Update(BASTAssigneeUpdateDto input);
        void SoftDelete(Guid id);
    }
}
