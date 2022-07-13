using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IGuideTechnicalCategoryAppService : IApplicationService
    {
        BaseResponse GetAllAdmin([FromQuery] Pagination request);
        List<Guid> GetAllIds();
        GuideTechnicalCategories GetById(Guid id);
        void Create(GuideTechnicalCategories input);
        void Update(GuideTechnicalCategories input);
        void SoftDelete(Guid id, string username);
    }
}
