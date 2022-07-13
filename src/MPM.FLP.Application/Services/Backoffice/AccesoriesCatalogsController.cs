using System.Collections.Generic;
using MPM.FLP.FLPDb;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MPM.FLP.Services.Backoffice
{
    public class AccesoriesCatalogsController : FLPAppServiceBase, IAccesoriesCatalogsController
    {
        private readonly AccesoriesCatalogAppService _appService;

        public AccesoriesCatalogsController(AccesoriesCatalogAppService appService)
        {
            _appService = appService;
        }

        [HttpGet("/api/services/app/backoffice/AccesoriesCatalogs/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query =_appService.GetAll();

            var count = query.Count();

            if(!string.IsNullOrEmpty(request.Query)){
                query = query.Where(x=> x.Title.Contains(request.Query) || x.CreatorUsername.Contains(request.Query) || x.AccessoriesCode.Contains(request.Query) || x.Price.ToString().Contains(request.Query));
            }

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/AccesoriesCatalogs/getByID")]
        public AccesoriesCatalogs GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/AccesoriesCatalogs/create")]
        public async Task<AccesoriesCatalogs> CreateBackoffice([FromBody]AccesoriesCatalogs model, [FromBody]IEnumerable<IFormFile> files)
        {
            if (model != null)
            {
                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.Now;
                model.CreatorUsername = "admin";
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;
                model.DeleterUsername = "";
                model.FeaturedImageUrl = "";
                if (files.Count() > 0)
                {
                    AzureController azureController = new AzureController();
                    foreach (var file in files)
                    {
                        model.FeaturedImageUrl = await azureController.InsertAndGetUrlAzure(file, model.Id.ToString(), "IMG", "accesoriescatalog");
                    }
                }

                _appService.Create(model);
            };

            return model;
        }

        [HttpPut("/api/services/app/backoffice/AccesoriesCatalogs/update")]
        public async Task<AccesoriesCatalogs> UpdateBackoffice(AccesoriesCatalogs model, IEnumerable<IFormFile> files)
        {
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;

                if (files.Count() > 0)
                {
                    AzureController azureController = new AzureController();
                    foreach (var file in files)
                    {
                        model.FeaturedImageUrl = await azureController.InsertAndGetUrlAzure(file, model.Id.ToString(), "IMG", "accesoriescatalog");
                    }
                }
                _appService.Update(model);
            }
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/AccesoriesCatalogs/destroy")]
        public String DestroyBackoffice(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }
    }
}