using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Authorization;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class ProductPriceAppService : FLPAppServiceBase, IProductPriceAppService
    {
        private readonly IRepository<ProductPrices, Guid> _productPricesRepository;

        public ProductPriceAppService(IRepository<ProductPrices, Guid> productPricesRepository)
        {
            _productPricesRepository = productPricesRepository;
        }

        public IQueryable<ProductPrices> GetAll()
        {
            return _productPricesRepository.GetAll().Include(y => y.ProductColorVariants);
        }

        public ProductPrices GetById(Guid id)
        {
            var productPricess = _productPricesRepository.FirstOrDefault(x => x.Id == id);

            return productPricess;
        }
        public ProductPrices GetByDealerAndColorVariant(string kodeDealer, Guid colorVariantId) 
        {
            var productPricess = _productPricesRepository.FirstOrDefault(x => x.KodeDealerMPM == kodeDealer && x.ProductColorVariantId == colorVariantId);

            return productPricess;
        }

        public void Create(ProductPrices input)
        {
            _productPricesRepository.Insert(input);
        }

        public void Update(ProductPrices input)
        {
            _productPricesRepository.Update(input);
        }
        
    }
}
