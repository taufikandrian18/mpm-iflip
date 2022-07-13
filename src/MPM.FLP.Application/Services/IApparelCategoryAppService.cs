using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IApparelCategoryAppService : IApplicationService
    {
        IQueryable<ApparelCategories> GetAll();
        List<ApparelCategories> GetAllApparelCategoryItems();
        List<Guid> GetAllIds();
        ApparelCategories GetById(Guid id);
        void Create(ApparelCategories input);
        void Update(ApparelCategories input);
        void SoftDelete(Guid id, string username);
    }
}
