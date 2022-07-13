using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IProductAccesoriesAppService : IApplicationService
    {
        IQueryable<ProductAccesories> GetAll();
        List<Guid> GetAllIds();
        List<Guid> GetAllIdsByCatalogProduct(Guid catalogProductId);
        ProductAccesories GetById(Guid id);
        void Create(ProductAccesories input);
        void Update(ProductAccesories input);
        void SoftDelete(Guid Id, string username);
    }
}
