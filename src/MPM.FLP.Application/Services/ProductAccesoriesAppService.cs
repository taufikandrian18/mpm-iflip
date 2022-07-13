using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Authorization;
using MPM.FLP.Authorization.Users;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class ProductAccesoriesAppService : FLPAppServiceBase, IProductAccesoriesAppService
    {
        private readonly IRepository<ProductAccesories, Guid> _productAccesoriesRepository;

        public ProductAccesoriesAppService(IRepository<ProductAccesories, Guid> productAccesoriesRepository)
        {
            _productAccesoriesRepository = productAccesoriesRepository;
        }

        public IQueryable<ProductAccesories> GetAll()
        {
            return _productAccesoriesRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername));
        }

        public List<Guid> GetAllIds()
        {
            return _productAccesoriesRepository.GetAll().Select(x => x.Id).ToList();
        }


        public List<Guid> GetAllIdsByCatalogProduct(Guid catalogProductId)
        {
            var productAccesoriess = _productAccesoriesRepository.GetAll().Where(x => x.ProductCatalogId == catalogProductId
                                                            && string.IsNullOrEmpty(x.DeleterUsername));

            return productAccesoriess.Select(x => x.Id).ToList();
        }

        public ProductAccesories GetById(Guid id)
        {
            var productAccesoriess = _productAccesoriesRepository.GetAll().FirstOrDefault(x => x.Id == id);

            return productAccesoriess;
        }

        public void Create(ProductAccesories input)
        {
            _productAccesoriesRepository.Insert(input);
        }

        public void Update(ProductAccesories input)
        {
            _productAccesoriesRepository.Update(input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var productAccesories = _productAccesoriesRepository.FirstOrDefault(x => x.Id == id);
            productAccesories.DeleterUsername = username;
            productAccesories.DeletionTime = DateTime.Now;
            _productAccesoriesRepository.Update(productAccesories);
        }
    }
}
