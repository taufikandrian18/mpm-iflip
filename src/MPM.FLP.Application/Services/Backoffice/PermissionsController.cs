using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.Services.Backoffice
{
    public class PermissionsController : FLPAppServiceBase, IPermissionsController
    {
        private readonly PermissionsAppService _appService;

        public PermissionsController(PermissionsAppService appService)
        {
            _appService = appService;
        }

        [HttpGet("/api/services/app/backoffice/Permissions/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery]Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _appService.GetAll();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Code.Contains(request.Query) || x.Name.Contains(request.Query));
            }

            var count = query.Count();

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/Permissions/getByID")]
        public TBPermissions GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/Permissions/create")]
        public async Task<TBPermissions> CreateBackoffice([FromForm]PermissionsVM data)
        {
            TBPermissions model = new TBPermissions();
            model.Id = Guid.NewGuid();
            model.CreationTime = DateTime.Now;
            model.CreatorUsername = "admin";
            model.LastModifierUsername = "admin";
            model.LastModificationTime = DateTime.Now;
            model.DeleterUsername = "";
            model.Code = data.Code;
            model.Name = data.Name;

            _appService.Create(model);

            return model;
        }

        [HttpPut("/api/services/app/backoffice/Permissions/update")]
        public async Task<TBPermissions> EditBackoffice([FromForm]PermissionsVM data)
        {
            TBPermissions model = _appService.GetById(data.id);
            model.LastModifierUsername = "admin";
            model.LastModificationTime = DateTime.Now;
            model.Code = data.Code;
            model.Name = data.Name;

            _appService.Update(model);
            return model;
        }


        [HttpDelete("/api/services/app/backoffice/Permissions/destroy")]
        public String DestroyBackoffice(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }
    }
}