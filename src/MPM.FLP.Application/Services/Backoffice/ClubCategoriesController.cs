using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MPM.FLP.Services.Backoffice
{
    public class ClubCategoriesController : FLPAppServiceBase, IClubCategoriesController
    {
        private readonly ClubCommunityCategoryAppService _appService;

        public ClubCategoriesController(ClubCommunityCategoryAppService appService)
        {
            _appService = appService;
        }

        [HttpGet("/api/services/app/backoffice/ClubCategories/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);
            var query = _appService.GetAll();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => 
                    x.CreatorUsername.Contains(request.Query) ||
                    x.Name.Contains(request.Query) ||
                    x.Order.ToString() == request.Query
                );
            }
            var count = query.Count();
            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/ClubCategories/getByID")]
        public ClubCommunityCategories GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/ClubCategories/create")]
        public async Task<ClubCommunityCategories> Create([FromForm]ClubCommunityCategories model, [FromForm]IEnumerable<IFormFile> images)
        {
            if (model != null)
            {
                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.Now;
                model.CreatorUsername = "admin";
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;
                model.DeleterUsername = "";

                if (images.Count() > 0)
                {
                    AzureController azureController = new AzureController();
                    //model.IconUrl = await InsertToAzure(files.FirstOrDefault(), model);
                    model.IconUrl = await azureController.InsertAndGetUrlAzure(images.FirstOrDefault(), model.Id.ToString(), "IMG", "clubcategories");
                }

                _appService.Create(model);

            };

            return model;
        }


        [HttpPut("/api/services/app/backoffice/ClubCategories/update")]
        public async Task<ClubCommunityCategories> Edit(ClubCommunityCategories model, IEnumerable<IFormFile> images)
        {
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;

                if (images.Count() > 0)
                {
                    AzureController azureController = new AzureController();
                    //model.IconUrl = await InsertToAzure(files.FirstOrDefault(), model);
                    model.IconUrl = await azureController.InsertAndGetUrlAzure(images.FirstOrDefault(), model.Id.ToString(), "IMG", "clubcategories");
                }

                _appService.Update(model);
            }
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/ClubCategories/destroy")]
        public String DestroyBackoffice(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }

    }
}
