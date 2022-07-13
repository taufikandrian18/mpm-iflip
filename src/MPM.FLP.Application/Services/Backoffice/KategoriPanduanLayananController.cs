using MPM.FLP.Services;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace MPM.FLP.Services.Backoffice
{
    public class KategoriPanduanLayananController : FLPAppServiceBase, IKategoriPanduanLayananController
    {
        private readonly GuideCategoryAppService _appService;

        public KategoriPanduanLayananController(GuideCategoryAppService guideCategoryAppService)
        {
            _appService = guideCategoryAppService;
        }

        [HttpGet("/api/services/app/backoffice/KategoriPanduanLayanan/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _appService.GetAll();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Name.Contains(request.Query) || x.CreatorUsername.Contains(request.Query));
            }

            var count = query.Count();

            var data = query.OrderBy(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/KategoriPanduanLayanan/getByID")]
        public GuideCategories GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/KategoriPanduanLayanan/create")]
        public GuideCategories Create([FromForm]GuideCategories model)
        {
            if(model != null)
            {
                GuideCategories guideCategories = new GuideCategories
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Order = model.Order,
                    CreationTime = DateTime.Now,
                    CreatorUsername = "admin",
                    IsPublished = model.IsPublished,
                    LastModifierUsername = "admin",
                    LastModificationTime = DateTime.Now,
                    DeleterUsername = ""
                };

                _appService.Create(guideCategories);
            }
            return model;
        }

        [HttpPut("/api/services/app/backoffice/KategoriPanduanLayanan/update")]
        public GuideCategories Edit(GuideCategories model)
        {
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;

                _appService.Update(model);
            }
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/KategoriPanduanLayanan/destroy")]
        public String Destroy(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }
    }
}