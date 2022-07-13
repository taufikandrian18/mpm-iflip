using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IProductSeriesAppService : IApplicationService
    {
        BaseResponse GetAll([FromQuery] Pagination request);
        ProductSeries GetById(Guid Id);
        void Create(ProductSeriesCreateDto input);
        void Update(ProductSeriesUpdateDto input);
        void SoftDelete(ProductSeriesDeleteDto input);
        Task<ServiceResult> SyncProductType(string itemGroupName);
        Task<ServiceResult> SyncProductSeries(string itemGroupName);
    }
}
