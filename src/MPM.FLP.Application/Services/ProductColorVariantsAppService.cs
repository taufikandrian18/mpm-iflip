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
    public class ProductColorVariantsAppService : FLPAppServiceBase, IProductColorVariantsAppService
    {
        private readonly IRepository<ProductColorVariants, Guid> _productColorVariantsRepository;

        public ProductColorVariantsAppService(IRepository<ProductColorVariants, Guid> productColorVariantsRepository)
        {
            _productColorVariantsRepository = productColorVariantsRepository;
        }

        public IQueryable<ProductColorVariants> GetAll()
        {
            return _productColorVariantsRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername));
        }

        public List<Guid> GetAllIds()
        {
            return _productColorVariantsRepository.GetAll().Select(x => x.Id).ToList();
        }


        public List<Guid> GetAllIdsByCatalogProduct(Guid catalogProductId)
        {
            var productColorVariantss = _productColorVariantsRepository.GetAll().Where(x => x.ProductCatalogId == catalogProductId
                                                            && string.IsNullOrEmpty(x.DeleterUsername));

            return productColorVariantss.Select(x => x.Id).ToList();
        }

        public ProductColorVariants GetById(Guid id)
        {
            var productColorVariants = _productColorVariantsRepository.GetAll().FirstOrDefault(x => x.Id == id);

            return productColorVariants;
        }

        public void Create(ProductColorVariants input)
        {
            _productColorVariantsRepository.Insert(input);
        }

        public void Update(ProductColorVariants input)
        {
            _productColorVariantsRepository.Update(input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var productColorVariants = _productColorVariantsRepository.FirstOrDefault(x => x.Id == id);
            productColorVariants.DeleterUsername = username;
            productColorVariants.DeletionTime = DateTime.Now;
            _productColorVariantsRepository.Update(productColorVariants);
        }
    }
}
