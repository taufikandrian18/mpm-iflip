using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;

namespace MPM.FLP.Services
{
    public interface IContentBankAppService : IApplicationService
    {
        BaseResponse GetAll([FromQuery] Pagination request);
        List<ContentBankResponse> GetByCategory(Guid ContentBankCategoryId, bool? isH1, bool? isH2, bool? isH3);
        void Create(ContentBanksCreateDto input);
        void Update(ContentBanksUpdateDto input);
        void SoftDelete(ContentBanksDeleteDto input);
    }
}
