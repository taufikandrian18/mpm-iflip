﻿using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;

namespace MPM.FLP.Services
{
    public interface IBASTsAppService : IApplicationService
    {
        BaseResponse GetAll([FromQuery] Pagination request);
        BASTs GetById(Guid Id);
        void Create(BASTCreateDto input);
    }
}
