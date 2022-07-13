using Abp.Domain.Repositories;
using CorePush.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public class ProductTypeAppService : FLPAppServiceBase, IProductTypeAppService
    {
        private readonly IRepository<ProductTypes, Guid> _repositoryProductTypes;
        private readonly IRepository<ProductSeries, Guid> _repositoryProductSeries;
        public ProductTypeAppService(
            IRepository<ProductTypes, Guid> repositoryProductTypes,
            IRepository<ProductSeries, Guid> repositoryProductSeries)
        {
            _repositoryProductTypes = repositoryProductTypes;
            _repositoryProductSeries = repositoryProductSeries;
        }

        public BaseResponse GetAll([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _repositoryProductTypes.GetAll().Where(x => x.DeletionTime == null);
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.ProductName.Contains(request.Query));
            }

            var count = query.Count();
            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        public ProductTypes GetById(Guid Id)
        {
            var data = _repositoryProductTypes.GetAll()
                        .FirstOrDefault(x => x.Id == Id);
            return data;
        }

        public void Create(ProductTypesCreateDto input)
        {
            #region Create Product Types
            var productType = ObjectMapper.Map<ProductTypes>(input);
            productType.CreationTime = DateTime.Now;
            productType.CreatorUsername = this.AbpSession.UserId.ToString();

            _repositoryProductTypes.Insert(productType);
            #endregion
        }
        public void Update(ProductTypesUpdateDto input)
        {
            #region Update Product Types
            var productType = _repositoryProductTypes.Get(input.Id);
            productType.ProductName = input.ProductName;
            productType.ProductCode = input.ProductCode;
            productType.LastModifierUsername = this.AbpSession.UserId.ToString();
            productType.LastModificationTime = DateTime.Now;

            _repositoryProductTypes.Update(productType);
            #endregion

        }
        public void SoftDelete(ProductTypesDeleteDto input)
        {
            var productType = _repositoryProductTypes.Get(input.Id);
            productType.DeleterUsername = this.AbpSession.UserId.ToString();
            productType.DeletionTime = DateTime.Now;
            _repositoryProductTypes.Update(productType);

            SoftDeleteProductSeries(input.Id);
        }
        private void SoftDeleteProductSeries(Guid ProductTypeId)
        {
            var series = _repositoryProductSeries.GetAllList(x => x.GUIDProductType == ProductTypeId);
            foreach (var seri in series)
            {
                seri.DeleterUsername = this.AbpSession.UserId.ToString();
                seri.DeletionTime = DateTime.Now;
                _repositoryProductSeries.Update(seri);
            }
        }
    }
}
