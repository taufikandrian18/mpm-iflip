using Abp.Application.Services;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IProductCatalogAppService : IApplicationService
    {
        IQueryable<ProductCatalogs> GetAll();
        IQueryable<ProductCatalogs> GetAllBackoffice();
        //ICollection<ProductCatalogAttachments> GetAllAttachments(Guid id);
        BaseResponse GetAllAttachments(Pagination request);
        List<Guid> GetAllIds();
        List<Guid> GetAllIdsByCategories(Guid categoryId);
        ProductCatalogs GetByIdAdmin(Guid id);
        ProductCatalogs GetById(Guid id);
        void Create(ProductCatalogs input);
        void Update(ProductCatalogs input);
        void SoftDelete(Guid Id, string username);
    }
}
