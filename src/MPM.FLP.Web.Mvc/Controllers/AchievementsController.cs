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

namespace MPM.FLP.Web.Mvc.Controllers
{
    public class AchievementsController : FLPControllerBase
    {
        private readonly AchievementAppService _appService;

        public AchievementsController(AchievementAppService appService)
        {
            _appService = appService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            Achievements model = new Achievements();
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(Achievements model, string submit)
        {
            if(model != null)
            {
                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.Now;
                model.CreatorUsername = this.User.Identity.Name;
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.Now;
                model.DeleterUsername = "";

                _appService.Create(model);
            }
            return View(model);
        }

        public IActionResult Edit(Guid id)
        {
            //var item = _appService.Get(id);

            //return View(item);

            return View();
        }

        [HttpPost]
        public IActionResult Edit(Achievements model, string submit)
        {
            if (model != null)
            {
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.Now;

                //_pointsAppService.Update(model);
            }
            return View(model);
        }

        public IActionResult Grid_Read([DataSourceRequest]DataSourceRequest request)
        {

            DataSourceResult result = _appService.GetAll().ToDataSourceResult(request);

            return Json(result);
        }

        public IActionResult Grid_Destroy([DataSourceRequest]DataSourceRequest request, Achievements item)
        {
            if (ModelState.IsValid)
            {
                //_pointsAppService.Delete(item.Id, item.Name);
            }
            
            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }
    }
}