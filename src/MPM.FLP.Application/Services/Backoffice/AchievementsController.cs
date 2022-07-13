using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using System;
using System.Linq;

namespace MPM.FLP.Services.Backoffice
{
    public class AchievementsController : FLPAppServiceBase, IAchievementsController
    {
        private readonly AchievementAppService _appService;

        public AchievementsController(AchievementAppService appService)
        {
            _appService = appService;
        }

        [HttpGet("/api/services/app/backoffice/Achievements/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query =_appService.GetAll();

            var count = query.Count();

            if(!string.IsNullOrEmpty(request.Query)){
                query = query.Where(x=> x.CreatorUsername.Contains(request.Query) || x.Description.Contains(request.Query) || x.Name.Contains(request.Query));
            }

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/Achievements/getByID")]
        public Achievements GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/Achievements/create")]
        public Achievements CreateBackoffice([FromBody]Achievements model)
        {
            if(model != null)
            {
                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.Now;
                model.CreatorUsername = "admin";
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;
                model.DeleterUsername = "";

                _appService.Create(model);
            }
            return model;
        }

        [HttpPut("/api/services/app/backoffice/Achievements/update")]
        public Achievements UpdateBackoffice(Achievements model)
        {
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;

                _appService.Update(model);
            }
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/Achievements/destroy")]
        public String DestroyBackoffice(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }
    }
}