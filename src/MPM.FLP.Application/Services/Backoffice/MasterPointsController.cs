using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using System;
using System.Linq;

namespace MPM.FLP.Services.Backoffice
{
    public class MasterPointsController : FLPAppServiceBase, IMasterPointsController
    {
        private readonly SalesPeopleDevelopmentContestAppService _appService;

        public MasterPointsController(SalesPeopleDevelopmentContestAppService pointsAppService)
        {
            _appService = pointsAppService;
        }

        [HttpGet("/api/services/app/backoffice/MasterPoints/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _appService.GetAllMasterPoint();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Title.Contains(request.Query) || x.Weight.ToString().Contains(request.Query));
            }

            var count = query.Count();

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpPost("/api/services/app/backoffice/MasterPoints/create")]
        public string Create(SPDCMasterPoints model)
        {
            var totalNow = _appService.GetAllMasterPoint().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).Sum(x => x.Weight);
            var totalReal = totalNow + model.Weight;
            
            if (totalReal > 1)
            {
                return "Total Real must be lower than 1";
            }

            if (model != null)
            {
                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.Now;
                model.CreatorUsername = "admin";
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;
                model.DeleterUsername = "";

                _appService.CreateMasterPoint(model);
                return "Success";
            }
            return "Something went wrong";
        }

        [HttpPut("/api/services/app/backoffice/MasterPoints/update")]
        public string Update(SPDCMasterPoints model)
        {
            var totalBefore = _appService.GetAllMasterPoint().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).Sum(x => x.Weight);
            var valueBefore = _appService.GetAllMasterPoint().Where(x => x.Id == model.Id).Select(x => x.Weight).SingleOrDefault();
            var totalNow = totalBefore - valueBefore;
            var totalReal = totalNow + model.Weight;

            if(totalReal > 1)
            {
                return "Total Real must be lower than 1";
            }
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;

                _appService.UpdateMasterHistory(model);

                return "Success";
            }
            return "Something went wrong";
        }

        [HttpDelete("/api/services/app/backoffice/MasterPoints/destroy")]
        public string DestroyBackoffice(Guid guid)
        {
            _appService.SoftDeleteMasterPoint(guid, "admin");
            return "Successfully deleted";

        }
    }
}