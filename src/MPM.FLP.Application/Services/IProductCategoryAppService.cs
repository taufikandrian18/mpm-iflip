using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IProductCategoryAppService : IApplicationService
    {
        IQueryable<ProductCategories> GetAll();
        List<ProductCategories> GetAllProductCategoryItems(string query);

        List<ProductCatalogs> GetAllProductCatalogs(string query, string categoryId);

        List<Guid> GetAllIds();
        ProductCategories GetById(Guid id);
        void Create(ProductCategories input);
        void Update(ProductCategories input);
        void SoftDelete(Guid id, string username);
    }
}
