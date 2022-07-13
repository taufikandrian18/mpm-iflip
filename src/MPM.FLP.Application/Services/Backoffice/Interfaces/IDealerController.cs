using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

namespace MPM.FLP.Services.Backoffice
{
    public interface IDealerController : IApplicationService
    {
        BaseResponse GetKota(Pagination request);
        BaseResponse GetChannel(Pagination request);
        BaseResponse GetDealer(Pagination request);

        BaseResponse GetKerasidenan(Pagination request);


    }
}