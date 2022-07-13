using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface ISalesTargetAppService : IApplicationService
    {
        void Create(TargetSales input);
        void CreateMultiple(List<TargetSales> input);
        BaseResponse GetAll([FromQuery] Pagination request);
        TargetSales GetById(Guid Id);
        void CreateSingle(TargetSalesCreateDto input);
        void Update(TargetSalesUpdateDto input);
        void SoftDelete(ProductSeriesDeleteDto input);
        Task<ServiceResult> SyncSales(SalesParamDto input);
    }
}
