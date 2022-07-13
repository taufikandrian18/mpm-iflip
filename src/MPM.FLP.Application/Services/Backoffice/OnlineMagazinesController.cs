using Microsoft.AspNetCore.Mvc;
using MPM.FLP.Services;
using System.Collections.Generic;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using MPM.FLP.Authorization.Users;

namespace MPM.FLP.Services.Backoffice
{
    public class OnlinemagazinesController : FLPAppServiceBase, IOnlinemagazinesController
    {
        private readonly OnlineMagazineAppService _appService;

        public OnlinemagazinesController(OnlineMagazineAppService appService)
        {
            _appService = appService;
        }

        [HttpGet("/api/services/app/backoffice/Onlinemagazines/getAll")]
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

        [HttpGet("/api/services/app/backoffice/Onlinemagazines/getByID")]
        public OnlineMagazines GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/Onlinemagazines/create")]
        public async Task<OnlineMagazines> Create([FromForm]OnlineMagazines model, [FromForm]IEnumerable<IFormFile> files, [FromForm]IEnumerable<IFormFile> images)
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
                model.CoverUrl = "";

                AzureController azureController = new AzureController();
                foreach (var image in images)
                {
                    model.CoverUrl = await azureController.InsertAndGetUrlAzure(image, model.Id.ToString(), "IMG", "onlinemagazines");
                }
                foreach (var file in files)
                {
                    model.StorageUrl = await azureController.InsertAndGetUrlAzure(file, model.Id.ToString(), "DOC", "onlinemagazines");
                }

                _appService.Create(model);

            };

            return model;
        }

        [HttpPut("/api/services/app/backoffice/Onlinemagazines/create")]
        public async Task<OnlineMagazines> Edit(OnlineMagazines model, IEnumerable<IFormFile> files, IEnumerable<IFormFile> images)
        {
            if (model != null)
            {
                if(model.CoverUrl == null)
                {
                    model.CoverUrl = "";
                }
                if (model.StorageUrl == null)
                {
                    model.StorageUrl = "";
                }
                AzureController azureController = new AzureController();
                if (images.Count() > 0)
                {
                    model.CoverUrl = await azureController.InsertAndGetUrlAzure(images.FirstOrDefault(), model.Id.ToString(), "IMG", "motivationcards");
                }
                if (files.Count() > 0)
                {
                    model.StorageUrl = await azureController.InsertAndGetUrlAzure(files.FirstOrDefault(), model.Id.ToString(), "DOC", "motivationcards");
                }
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;

                _appService.Update(model);
            }
            return model;
        }


        [HttpDelete("/api/services/app/backoffice/Onlinemagazines/destroy")]
        public String Destroy(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }
    }
}