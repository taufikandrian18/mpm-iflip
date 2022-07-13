using Microsoft.AspNetCore.Mvc;
using Abp.AspNetCore.Mvc.Authorization;
using MPM.FLP.Controllers;
using MPM.FLP.Services;
using System.Collections;
using System.Collections.Generic;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using MPM.FLP.EntityFrameworkCore;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using System.Threading.Tasks;
using MPM.FLP.Services.Dto;

namespace MPM.FLP.Web.Mvc.Controllers
{
    public class PointsController : FLPControllerBase
    {
        private readonly IPointAppService _pointsAppService;

        public PointsController(IPointAppService pointsAppService)
        {
            _pointsAppService = pointsAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            AddPointConfigurationDto model = new AddPointConfigurationDto();
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(AddPointConfigurationDto model, string submit)
        {
            if(model != null)
            {
                //model.Id = Guid.NewGuid();
                //model.CreationTime = DateTime.Now;
                //model.CreatorUsername = this.User.Identity.Name;
                //model.LastModifierUsername = this.User.Identity.Name;
                //model.LastModificationTime = DateTime.Now;
                //model.DeleterUsername = "";

                _pointsAppService.AddPointConfiguration(model);
            }
            return View(model);
        }

        public IActionResult Edit(Guid id)
        {
            var item = _pointsAppService.GetPointConfigurationById(id);

            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(GuideCategories model, string submit)
        {
            if (model != null)
            {
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.Now;

                //_pointsAppService.Update(model);
            }
            return View(model);
        }

        public async Task<IActionResult> GuideCategory_Read([DataSourceRequest]DataSourceRequest request)
        {
            DataSourceResult result = (await _pointsAppService.GetActivePointConfigurations()).ToDataSourceResult(request);

            return Json(result);
        }

        public IActionResult GuideCategory_Destroy([DataSourceRequest]DataSourceRequest request, GuideCategories item)
        {
            if (ModelState.IsValid)
            {
                //_pointsAppService.Delete(item.Id, item.Name);
            }
            
            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }
    }
}