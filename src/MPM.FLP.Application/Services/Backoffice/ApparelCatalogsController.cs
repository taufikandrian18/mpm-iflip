using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MPM.FLP.Services.Backoffice
{
    public class ApparelCatalogsController : FLPAppServiceBase, IApparelCatalogsController
    {
        private readonly ApparelCatalogAppService _appService;

        public ApparelCatalogsController(ApparelCatalogAppService appService)
        {
            _appService = appService;
        }

        [HttpGet("/api/services/app/backoffice/ApparelCatalogs/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query =_appService.GetAllAdmin();

            if(!string.IsNullOrEmpty(request.Query)){
                query = query.Where(x=> x.ApparelCode.Contains(request.Query) || x.Title.Contains(request.Query) || x.Price.ToString().Contains(request.Query));
            }
            var count = query.Count();

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/ApparelCatalogs/getByID")]
        public ApparelCatalogs GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/ApparelCatalogs/create")]
        public async Task<ApparelCatalogs> CreateBackoffice([FromForm]ApparelCatalogsVM data, [FromForm]IEnumerable<IFormFile> files)
        {
            ApparelCatalogs model = new ApparelCatalogs();
            if (model != null)
            {
                model.Id = Guid.NewGuid();
                model.ApparelCategoryId = data.ApparelCategoryId;
                model.Order = data.Order;
                model.Title = data.Title;
                model.Price = data.Price;
                model.ApparelCode = data.ApparelCode;
                model.IsPublished = data.IsPublished;
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
                        model.FeaturedImageUrl = await azureController.InsertAndGetUrlAzure(file, model.Id.ToString(), "IMG", "apparelcatalog");
                    }
                }

                _appService.Create(model);
            };
            return model;
        }


        [HttpPut("/api/services/app/backoffice/ApparelCatalogs/update")]
        public async Task<ApparelCatalogs> UpdateBackoffice([FromForm]ApparelCatalogsVM data, [FromForm]IEnumerable<IFormFile> files)
        {
            ApparelCatalogs model = _appService.GetById(data.Id);
            model.ApparelCategoryId = data.ApparelCategoryId;
            model.Order = data.Order;
            model.Title = data.Title;
            model.Price = data.Price;
            model.ApparelCode = data.ApparelCode;
            model.IsPublished = data.IsPublished;
            model.LastModifierUsername = "admin";
            model.LastModificationTime = DateTime.Now;

            if (files.Count() > 0)
            {
                AzureController azureController = new AzureController();
                foreach (var file in files)
                {
                    model.FeaturedImageUrl = await azureController.InsertAndGetUrlAzure(file, model.Id.ToString(), "IMG", "apparelcatalog");
                }
            }
            _appService.Update(model);
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/ApparelCatalogs/destroy")]
        public String DestroyBackoffice(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }
    }
}
