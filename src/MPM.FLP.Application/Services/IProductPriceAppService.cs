using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IProductPriceAppService : IApplicationService
    {
        IQueryable<ProductPrices> GetAll();
        ProductPrices GetById(Guid id);
        ProductPrices GetByDealerAndColorVariant(string kodeDealer, Guid colorVariantId);
        void Create(ProductPrices input);
        void Update(ProductPrices input);
    }
}
