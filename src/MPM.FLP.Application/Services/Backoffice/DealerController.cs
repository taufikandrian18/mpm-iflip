using System.Collections.Generic;
using MPM.FLP.FLPDb;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MPM.FLP.Services.Backoffice
{
    public class DealerController : FLPAppServiceBase, IDealerController
    {
        private readonly DealerAppService _appService;

        public DealerController(DealerAppService appService)
        {
            _appService = appService;
        }

        [HttpGet("/api/services/app/backoffice/dealer/get-kota")]
        public BaseResponse GetKota([FromQuery] Pagination request)
        {
            var data =  new List<string>();
            if(request.Channel == "H1"){
                data = _appService.GetKotaH1(request.Channel);

            } else if(request.Channel == "H2") {
                data = _appService.GetKotaH2();

            } else if (request.Channel == "HC3"){
                data = _appService.GetKotaHC3(request.Channel, request.Kerasidenan);

            } else {
                data = _appService.GetKota();
            }

            var count = data.Count();
            return BaseResponse.Ok(data, count);        }

        [HttpGet("/api/services/app/backoffice/dealer/get-channel")]
        public BaseResponse GetChannel(Pagination request){
            var data = _appService.GetChannel();
            var count = data.Count();
            return BaseResponse.Ok(data, count);

        }

        [HttpGet("/api/services/app/backoffice/dealer/get-dealer")]
        public BaseResponse GetDealer(Pagination request){
            var data = _appService.GetDealersBackoffice(request.Kota);
            var count = data.Count();
            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/dealer/get-kerasidenan")]
        public BaseResponse GetKerasidenan(Pagination request){
            var data = new List<string>();
            if(request.Channel == "H1"){
                 data = _appService.GetKaresidenanH1();
            } else if(request.Channel == "HC3") {
                 data = _appService.GetKaresidenanHC3(request.Key);
            } else {
                 data = _appService.GetKaresidenan();
            }
            var count = data.Count();
            return BaseResponse.Ok(data, count);
        }

    }
}