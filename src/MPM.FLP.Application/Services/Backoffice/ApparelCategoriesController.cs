using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MPM.FLP.FLPDb;
using System;
using System.Linq;

namespace MPM.FLP.Services.Backoffice
{
    public class ApparelCategoriesController : FLPAppServiceBase, IApparelCategoriesController
    {
        private readonly ApparelCategoryAppService _appService;

        public ApparelCategoriesController(ApparelCategoryAppService appService)
        {
            _appService = appService;
        }

        [HttpGet("/api/services/app/backoffice/ApparelCategories/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query =_appService.GetAll();

            var count = query.Count();

            if(!string.IsNullOrEmpty(request.Query)){
                query = query.Where(x=> x.Name.Contains(request.Query) || x.CreatorUsername.Contains(request.Query));
            }

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/ApparelCategories/getByID")]
        public ApparelCategories GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/ApparelCategories/create")]
        public ApparelCategories CreateBackoffice([FromForm]ApparelCategoriesVM data)
        {
            ApparelCategories model = new ApparelCategories();
            if (model != null)
            {
                model.Id = Guid.NewGuid();
                model.Name = data.Name;
                model.IconUrl = data.IconUrl;
                model.Order = data.Order;
                model.IsPublished = data.IsPublished;
                model.CreationTime = DateTime.Now;
                model.CreatorUsername = "admin";
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;
                model.DeleterUsername = "";

                _appService.Create(model);
            };
            return model;
        }


        [HttpPut("/api/services/app/backoffice/ApparelCategories/update")]
        public ApparelCategories UpdateBackoffice([FromForm]ApparelCategoriesVM data)
        {
            ApparelCategories model = _appService.GetById(data.Id);
            if (model != null)
            {
                model.Name = data.Name;
                model.IconUrl = data.IconUrl;
                model.Order = data.Order;
                model.IsPublished = data.IsPublished;
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;
                _appService.Update(model);
            }
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/ApparelCategories/destroy")]
        public String DestroyBackoffice(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }
    }
}