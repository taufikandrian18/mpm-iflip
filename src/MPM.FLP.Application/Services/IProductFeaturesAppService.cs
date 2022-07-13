using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IProductFeaturesAppService : IApplicationService
    {
        IQueryable<ProductFeatures> GetAll();
        List<Guid> GetAllIds();
        List<Guid> GetAllIdsByCatalogProduct(Guid catalogProductId);
        ProductFeatures GetById(Guid id);
        void Create(ProductFeatures input);
        void Update(ProductFeatures input);
        void SoftDelete(Guid Id, string username);
    }
}
