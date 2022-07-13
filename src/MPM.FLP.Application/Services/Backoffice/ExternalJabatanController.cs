using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.Services.Backoffice
{
    public class ExternalJabatanController : FLPAppServiceBase, IExternalJabatanController
    {
        private readonly ExternalJabatanAppService _appService;

        public ExternalJabatanController(ExternalJabatanAppService appService)
        {
            _appService = appService;
        }

        [HttpGet("/api/services/app/backoffice/ExternalJabatan/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);
            var query = _appService.GetAll();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Nama.Contains(request.Query) || x.CreatorUsername.Contains(request.Query));
            }
            var count = query.Count();
            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/ExternalJabatan/getByID")]
        public ExternalJabatans GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/ExternalJabatan/create")]
        public async Task<ExternalJabatans> Create([FromForm]ExternalJabatans model)
        {
            if (model != null)
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

        [HttpPut("/api/services/app/backoffice/ExternalJabatan/update")]
        public ExternalJabatans Edit(ExternalJabatans model)
        {
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;

                _appService.Update(model);
            }
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/ExternalJabatan/destroy")]
        public String DestroyBackoffice(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }
    }
}
