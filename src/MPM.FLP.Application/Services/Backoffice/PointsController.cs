using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using System;
using MPM.FLP.Services.Dto;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.Services.Backoffice
{
    public class PointsController : FLPAppServiceBase, IPointsController
    {
        private readonly PointAppService _appService;

        public PointsController(PointAppService pointsAppService)
        {
            _appService = pointsAppService;
        }

        [HttpGet("/api/services/app/backoffice/Points/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _appService.GetAllBackoffice();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.ContentType.Contains(request.Query) || x.CreatorUsername.Contains(request.Query));
            }

            var count = query.Count();

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/Points/getByID")]
        public PointConfigurations GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/Points/create")]
        public AddPointConfigurationDto Create([FromForm]AddPointConfigurationDto model)
        {
            if(model != null)
            {
                _appService.AddPointConfiguration(model);
            }
            return model;
        }

        [HttpPut("/api/services/app/backoffice/Points/update")]
        public async Task<UpdatePointConfigurationDto> Edit(UpdatePointConfigurationDto model)
        {
            if (model != null)
            {
                model = await _appService.UpdatePointConfiguration(model);
            }
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/Points/destroy")]
        public async Task<String> Destroy(Guid guid)
        {
            await _appService.DeletePointConfiguration(guid);
            return "Successfully deleted";
        }
    }
}