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
    public class MasterPointsController : FLPControllerBase
    {
        private readonly SalesPeopleDevelopmentContestAppService _appService;

        public MasterPointsController(SalesPeopleDevelopmentContestAppService pointsAppService)
        {
            _appService = pointsAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Grid_Read([DataSourceRequest]DataSourceRequest request)
        {
            DataSourceResult result = _appService.GetAllMasterPoint().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderByDescending(x => x.CreationTime).ToDataSourceResult(request);

            return Json(result);
        }

        public IActionResult Grid_Destroy([DataSourceRequest]DataSourceRequest request, SPDCMasterPoints item)
        {
            if (ModelState.IsValid)
            {
                _appService.SoftDeleteMasterPoint(item.Id, this.User.Identity.Name);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        public IActionResult PointHistories(Guid id)
        {
            SPDCMasterPoints model = new SPDCMasterPoints();
            return View(model);
        }

        public IActionResult Create()
        {
            SPDCMasterPoints model = new SPDCMasterPoints();
            var total = _appService.GetAllMasterPoint().Where(x=>string.IsNullOrEmpty(x.DeleterUsername)).Sum(x => x.Weight);
            TempData["total"] = total;
            if (total == 1)
            {
                TempData["alert"] = "Total nilai tidak dapat melebihi 1";
                TempData["success"] = "";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(SPDCMasterPoints model, string submit)
        {
            var totalNow = _appService.GetAllMasterPoint().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).Sum(x => x.Weight);
            var totalReal = totalNow + model.Weight;
            
            if (totalReal > 1)
            {
                return Json(new { success = false });
            }
            else
            {
                if (model != null)
                {
                    model.Id = Guid.NewGuid();
                    model.CreationTime = DateTime.Now;
                    model.CreatorUsername = this.User.Identity.Name;
                    model.LastModifierUsername = this.User.Identity.Name;
                    model.LastModificationTime = DateTime.Now;
                    model.DeleterUsername = "";

                    _appService.CreateMasterPoint(model);
                }
                return Json(new { success = true });
            }
            
        }

        public IActionResult Edit(Guid id)
        {
            var item = _appService.GetAllMasterPoint().Where(x=>x.Id == id).SingleOrDefault();
            var total = _appService.GetAllMasterPoint().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).Sum(x => x.Weight);
            TempData["total"] = total;
            return View(item);

            //return View();
        }

        [HttpPost]
        public IActionResult Edit(SPDCMasterPoints model, string submit)
        {
            var totalBefore = _appService.GetAllMasterPoint().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).Sum(x => x.Weight);
            var valueBefore = _appService.GetAllMasterPoint().Where(x => x.Id == model.Id).Select(x => x.Weight).SingleOrDefault();
            var totalNow = totalBefore - valueBefore;
            var totalReal = totalNow + model.Weight;

            if(totalReal > 1)
            {
                return Json(new { success = false });
            }
            else
            {
                if (model != null)
                {
                    model.LastModifierUsername = this.User.Identity.Name;
                    model.LastModificationTime = DateTime.Now;

                    _appService.UpdateMasterHistory(model);

                    return Json(new { success = true });
                }
            }
            
            return View(model);
        }

    }
}