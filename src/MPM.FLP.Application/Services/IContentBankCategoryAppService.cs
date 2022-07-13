using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;

namespace MPM.FLP.Services
{
    public interface IContentBankCategoryAppService : IApplicationService
    {
        BaseResponse GetAll([FromQuery] Pagination request);
        void Create(ContentBankCategoriesCreateDto input);
        void Update(ContentBankCategoriesUpdateDto input);
        void SoftDelete(ContentBankCategoriesDeleteDto input);
    }
}
