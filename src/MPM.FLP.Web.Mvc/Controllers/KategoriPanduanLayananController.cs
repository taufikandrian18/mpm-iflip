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
    [AbpMvcAuthorize]
    public class KategoriPanduanLayananController : FLPControllerBase
    {
        private readonly GuideCategoryAppService _guideCategoryAppService;

        public KategoriPanduanLayananController(GuideCategoryAppService guideCategoryAppService)
        {
            _guideCategoryAppService = guideCategoryAppService;
        }

        public IActionResult Index()
        {
            TempData["alert"] = "";
            TempData["success"] = "";
            return View();
        }

        //public IActionResult Create()
        //{
        //    GuideCategories model = new GuideCategories();
        //    return View(model);
        //}
        public IActionResult Create(GuideCategories model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(GuideCategories model, string submit)
        {
            if(model != null)
            {
                if (model.Name == null)
                {
                    TempData["alert"] = "Nama masih kosong";
                    TempData["success"] = "";
                    return RedirectToAction("Create", model);
                }
                GuideCategories guideCategories = new GuideCategories
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Order = model.Order,
                    CreationTime = DateTime.Now,
                    CreatorUsername = this.User.Identity.Name,
                    IsPublished = model.IsPublished,
                    LastModifierUsername = this.User.Identity.Name,
                    LastModificationTime = DateTime.Now,
                    DeleterUsername = ""
                };

                _guideCategoryAppService.Create(guideCategories);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Edit(Guid id)
        {
            var item = _guideCategoryAppService.GetById(id);

            return View(item);

            //return null;
        }

        [HttpPost]
        public IActionResult Edit(GuideCategories model, string submit)
        {
            if (model != null)
            {
                if (model.Name == null)
                {
                    TempData["alert"] = "Judul masih kosong";
                    TempData["success"] = "";
                    return RedirectToAction("Edit", model.Id);
                }
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.Now;

                _guideCategoryAppService.Update(model);
            }
            return RedirectToAction("Index");
        }

        public IActionResult GuideCategory_Read([DataSourceRequest]DataSourceRequest request)
        {
            DataSourceResult result = _guideCategoryAppService.GetAll().Where(x=> string.IsNullOrEmpty(x.DeleterUsername)).ToDataSourceResult(request);

            return Json(result);
        }

        public IActionResult GuideCategory_Destroy([DataSourceRequest]DataSourceRequest request, GuideCategories item)
        {
            if (ModelState.IsValid)
            {
                _guideCategoryAppService.SoftDelete(item.Id, this.User.Identity.Name);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));

            //return null;
        }

        public IActionResult BackToIndex()
        {
            return RedirectToAction("Index");
        }
    }
}