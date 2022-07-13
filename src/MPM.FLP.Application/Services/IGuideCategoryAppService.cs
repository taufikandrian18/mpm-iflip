using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IGuideCategoryAppService : IApplicationService
    {
        IQueryable<GuideCategories> GetAll();
        List<Guid> GetAllIds();
        GuideCategories GetById(Guid id);
        void Create(GuideCategories input);
        void Update(GuideCategories input);
        void SoftDelete(Guid id, string username);
    }
}
