using Abp.Domain.Repositories;
using CorePush.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public class ProductSeriesAppService : FLPAppServiceBase, IProductSeriesAppService
    {
        private readonly IRepository<ProductTypes, Guid> _repositoryProductTypes;
        private readonly IRepository<ProductSeries, Guid> _repositoryProductSeries;
        public ProductSeriesAppService(
            IRepository<ProductTypes, Guid> repositoryProductTypes,
            IRepository<ProductSeries, Guid> repositoryProductSeries)
        {
            _repositoryProductTypes = repositoryProductTypes;
            _repositoryProductSeries = repositoryProductSeries;
        }

        public BaseResponse GetAll([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _repositoryProductSeries.GetAll().Where(x => x.DeletionTime == null);
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.SeriesName.Contains(request.Query));
            }

            var count = query.Count();
            var data = query.Include(x => x.ProductTypes).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        public ProductSeries GetById(Guid Id)
        {
            var data = _repositoryProductSeries.GetAll()
                        .Include(x => x.ProductTypes)
                        .FirstOrDefault(x => x.Id == Id);
            return data;
        }

        public void Create(ProductSeriesCreateDto input)
        {
            #region Create Product Series
            var productSeries = ObjectMapper.Map<ProductSeries>(input);
            productSeries.CreationTime = DateTime.Now;
            productSeries.CreatorUsername = this.AbpSession.UserId.ToString();

            _repositoryProductSeries.Insert(productSeries);
            #endregion
        }
        public void Update(ProductSeriesUpdateDto input)
        {
            #region Update product series
            var productSeries = _repositoryProductSeries.Get(input.Id);
            productSeries.GUIDProductType = input.GUIDProductType;
            productSeries.SeriesName = input.SeriesName;
            productSeries.SeriesCode = input.SeriesCode;
            productSeries.LastModifierUsername = this.AbpSession.UserId.ToString();
            productSeries.LastModificationTime = DateTime.Now;

            _repositoryProductSeries.Update(productSeries);
            #endregion

        }
        public void SoftDelete(ProductSeriesDeleteDto input)
        {
            var productSeries = _repositoryProductSeries.Get(input.Id);
            productSeries.DeleterUsername = this.AbpSession.UserId.ToString();
            productSeries.DeletionTime = DateTime.Now;
            _repositoryProductSeries.Update(productSeries);
        }
        public async Task<ServiceResult> SyncProductType(string itemGroupName)
        {
            try
            {
                var url = string.Format(AppConstants.MPMProductUrl, itemGroupName);

                var client = new HttpClient();
                
                var getProductResult = await client.GetAsync(url);
                var productJson = await getProductResult.Content.ReadAsStringAsync();

                MasterUnitResponseDto productResponse = JsonConvert.DeserializeObject<MasterUnitResponseDto>(productJson);
                //result = productResponse.data.ToList();
                if (productResponse.status == 1)
                {
                    //Sync Delete product type
                    var deletedProductType = _repositoryProductTypes.GetAll().Where(x => x.DeletionTime == null
                        && !productResponse.data.Select(y => y.KODETYPEUNITAHM).Contains(x.ProductCode)).ToList();
                    foreach (var productType in deletedProductType)
                    {
                        productType.DeleterUsername = "system";
                        productType.DeletionTime = DateTime.Now;
                        _repositoryProductTypes.Update(productType);
                    }

                    //Sync Insert and Update product type
                    foreach (var productType in productResponse.data)
                    {
                        var _productType = new ProductTypes
                        {
                            ProductCode = productType.KODETYPEUNITAHM,
                            ProductName = productType.namaunit,
                            CreationTime = DateTime.Now,
                            CreatorUsername = "system"
                        };
                        _repositoryProductTypes.InsertOrUpdate(_productType);
                    }
                    return new ServiceResult { IsSuccess = true, Message = "Sync Success" };
                }
                else
                {
                    return new ServiceResult { IsSuccess = false, Message = productResponse.message };
                }
            }
            catch (Exception ex)
            {
                
                return new ServiceResult { IsSuccess = false, Message = ex.Message };
            }

        }

        public async Task<ServiceResult> SyncProductSeries(string itemGroupName)
        {
            try
            {
                var url = string.Format(AppConstants.MPMProductUrl, itemGroupName);

                var client = new HttpClient();

                var getProductResult = await client.GetAsync(url);
                var productJson = await getProductResult.Content.ReadAsStringAsync();

                MasterUnitResponseDto productResponse = JsonConvert.DeserializeObject<MasterUnitResponseDto>(productJson);
                //result = productResponse.data.ToList();
                if (productResponse.status == 1)
                {
                    //Sync Delete product series
                    var deletedProductSeries = _repositoryProductSeries.GetAll().Where(x => x.DeletionTime == null
                        && !productResponse.data.Select(y => y.SERIES_UNIT).Contains(x.SeriesCode)).ToList();
                    foreach (var productSeries in deletedProductSeries)
                    {
                        productSeries.DeleterUsername = "system";
                        productSeries.DeletionTime = DateTime.Now;
                        _repositoryProductSeries.Update(productSeries);
                    }

                    //Sync Insert and Update product series
                    foreach (var productSeries in productResponse.data)
                    {
                        var tipeProduct = _repositoryProductTypes.GetAll()
                                            .Where(x => x.ProductCode == productSeries.KODETYPEUNITAHM)
                                            .Select(x => x.Id)
                                            .FirstOrDefault();
                        if (tipeProduct != null)
                        {
                            var IdProductType = tipeProduct;

                            var _productSeries = new ProductSeries
                            {
                                GUIDProductType = IdProductType,
                                SeriesCode = productSeries.SERIES_UNIT,
                                SeriesName = productSeries.SUBSERIES_UNIT,
                                CreationTime = DateTime.Now,
                                CreatorUsername = "system"
                            };
                            _repositoryProductSeries.InsertOrUpdate(_productSeries);
                        }

                    }
                    return new ServiceResult { IsSuccess = true, Message = "Sync Success" };
                }
                else
                {
                    return new ServiceResult { IsSuccess = false, Message = productResponse.message };
                }
            }
            catch (Exception ex)
            {

                return new ServiceResult { IsSuccess = false, Message = ex.Message };
            }

        }
    }
}
