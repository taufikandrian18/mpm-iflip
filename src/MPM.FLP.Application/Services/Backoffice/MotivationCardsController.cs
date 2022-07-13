using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MPM.FLP.Services.Backoffice
{
    public class MotivationCardsController : FLPAppServiceBase, IMotivationCardsController
    {
        private readonly MotivationCardAppService _appService;

        public MotivationCardsController(MotivationCardAppService appService)
        {
            _appService = appService;
        }

        [HttpGet("/api/services/app/backoffice/MotivationCards/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _appService.GetAll();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Title.Contains(request.Query) || x.CreatorUsername.Contains(request.Query));
            }

            var count = query.Count();

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/MotivationCards/getByID")]
        public MotivationCards GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/MotivationCards/create")]
        public async Task<MotivationCards> Create([FromForm]MotivationCards model, [FromForm]IEnumerable<IFormFile> files, [FromForm]IEnumerable<IFormFile> images)
        {
            if (model != null)
            {
                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.Now;
                model.CreatorUsername = "admin";
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;
                model.DeleterUsername = "";
                model.StorageUrl = "";

                foreach (var image in images)
                {
                    AzureController azureController = new AzureController();
                    model.StorageUrl = await azureController.InsertAndGetUrlAzure(image, model.Id.ToString(), "IMG", "motivationcards");
                }

                _appService.Create(model);

            };

            return model;
        }

        [HttpPut("/api/services/app/backoffice/MotivationCards/update")]
        public async Task<MotivationCards> Edit(MotivationCards model, IEnumerable<IFormFile> images)
        {
            if (model != null)
            {
                if(images.Count() > 0)
                {
                    AzureController azureController = new AzureController();
                    model.StorageUrl = await azureController.InsertAndGetUrlAzure(images.FirstOrDefault(), model.Id.ToString(), "IMG", "motivationcards");
                }
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;

                _appService.Update(model);
            }
            return model;
        }


        [HttpDelete("/api/services/app/backoffice/MotivationCards/destroy")]
        public String Destroy(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }
    }
}