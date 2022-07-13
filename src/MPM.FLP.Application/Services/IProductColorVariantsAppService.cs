using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IProductColorVariantsAppService : IApplicationService
    {
        IQueryable<ProductColorVariants> GetAll();
        List<Guid> GetAllIds();
        List<Guid> GetAllIdsByCatalogProduct(Guid catalogProductId);
        ProductColorVariants GetById(Guid id);
        void Create(ProductColorVariants input);
        void Update(ProductColorVariants input);
        void SoftDelete(Guid Id, string username);
    }
}
