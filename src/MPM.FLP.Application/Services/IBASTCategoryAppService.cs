using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;

namespace MPM.FLP.Services
{
    public interface IBASTCategoryAppService : IApplicationService
    {
        BaseResponse GetAll([FromQuery] Pagination request);
        BASTCategories GetById(Guid Id);
        void Create(BASTCategoryCreateDto input);
        void Update(BASTCategoryUpdateDto input);
        void SoftDelete(BASTCategoryDeleteDto input);
    }
}
