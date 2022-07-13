using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPM.FLP.Services
{
    public interface IApparelCatalogAppService : IApplicationService
    {
        IQueryable<ApparelCatalogs> GetAll();
        List<Guid> GetAllIds();
        List<Guid> GetAllIdsByCategories(Guid categoryId);
        ApparelCatalogs GetById(Guid id);
        void Create(ApparelCatalogs input);
        void Update(ApparelCatalogs input);
        void SoftDelete(Guid Id, string username);
    }
}
