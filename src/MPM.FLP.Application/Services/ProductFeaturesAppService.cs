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
    public class ProductFeaturesAppService : FLPAppServiceBase, IProductFeaturesAppService
    {
        private readonly IRepository<ProductFeatures, Guid> _productFeaturesRepository;

        public ProductFeaturesAppService(IRepository<ProductFeatures, Guid> productFeaturesRepository)
        {
            _productFeaturesRepository = productFeaturesRepository;
        }

        public IQueryable<ProductFeatures> GetAll()
        {
            return _productFeaturesRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername));
        }

        public List<Guid> GetAllIds()
        {
            return _productFeaturesRepository.GetAll().Select(x => x.Id).ToList();
        }


        public List<Guid> GetAllIdsByCatalogProduct(Guid catalogProductId)
        {
            var productFeaturess = _productFeaturesRepository.GetAll().Where(x => x.ProductCatalogId == catalogProductId
                                                            && string.IsNullOrEmpty(x.DeleterUsername));

            return productFeaturess.Select(x => x.Id).ToList();
        }

        public ProductFeatures GetById(Guid id)
        {
            var productFeaturess = _productFeaturesRepository.GetAll().FirstOrDefault(x => x.Id == id);

            return productFeaturess;
        }

        public void Create(ProductFeatures input)
        {
            _productFeaturesRepository.Insert(input);
        }

        public void Update(ProductFeatures input)
        {
            _productFeaturesRepository.Update(input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var productFeatures = _productFeaturesRepository.FirstOrDefault(x => x.Id == id);
            productFeatures.DeleterUsername = username;
            productFeatures.DeletionTime = DateTime.Now;
            _productFeaturesRepository.Update(productFeatures);
        }
    }
}
