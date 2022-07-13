using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;

namespace MPM.FLP.Services
{
    public interface IProductTypeAppService : IApplicationService
    {
        BaseResponse GetAll([FromQuery] Pagination request);
        ProductTypes GetById(Guid Id);
        void Create(ProductTypesCreateDto input);
        void Update(ProductTypesUpdateDto input);
        void SoftDelete(ProductTypesDeleteDto input);
    }
}
