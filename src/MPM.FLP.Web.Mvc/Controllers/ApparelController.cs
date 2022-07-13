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
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Threading.Tasks;
using MPM.FLP.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace MPM.FLP.Web.Mvc.Controllers
{
    [AbpMvcAuthorize]
    public class ApparelController : FLPControllerBase
    {
        private readonly ApparelCatalogAppService _appService;
        private readonly ApparelCategoryAppService _categorysAppService;

        public ApparelController(ApparelCatalogAppService appService, ApparelCategoryAppService categorysAppService)
        {
            _appService = appService;
            _categorysAppService = categorysAppService;
        }

        public IActionResult Index()
        {
            TempData["alert"] = "";
            TempData["success"] = "";
            return View();
        }

        //public IActionResult Create()
        //{
        //    ApparelCatalogs model = new ApparelCatalogs();
        //    return View(model);
        //}
        public IActionResult Create(ApparelCatalogs model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApparelCatalogs model, string submit, IEnumerable<IFormFile> files)
        {
            if (model != null)
            {
                if (model.Title == null)
                {
                    TempData["alert"] = "Nama masih kosong";
                    TempData["success"] = "";
                    return RedirectToAction("Create", model);
                }
                if (model.ApparelCategoryId == null)
                {
                    TempData["alert"] = "Kategori apparel masih kosong";
                    TempData["success"] = "";
                    return RedirectToAction("Create", model);
                }
                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.Now;
                model.CreatorUsername = this.User.Identity.Name;
                model.LastModifierUsername = this.User.Identity.Name;
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
                TempData["alert"] = "";
                TempData["success"] = "Berhasil menambahkan data";
            };

            return RedirectToAction("Index");
        }


        public IActionResult Edit(Guid id)
        {
            var item = _appService.GetById(id);

            return View(item);
            //return View();
        }


        [HttpPost]
        public async Task<IActionResult> Edit(ApparelCatalogs model, string submit, IEnumerable<IFormFile> files)
        {
            if (model != null)
            {
                if (model.Title == null)
                {
                    TempData["alert"] = "Nama masih kosong";
                    TempData["success"] = "";
                    return RedirectToAction("Edit", model.Id);
                }
                model.LastModifierUsername = this.User.Identity.Name;
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
            }
            return RedirectToAction("Index");
        }

        public IActionResult Grid_Read([DataSourceRequest]DataSourceRequest request)
        {
            var t = _appService.GetAllAdmin().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).ToList();
            DataSourceResult result = _appService.GetAllAdmin().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).ToDataSourceResult(request);

            return Json(result);
        }
        
        public IActionResult Grid_Destroy([DataSourceRequest]DataSourceRequest request, SalesTalks item)
        {
            if (ModelState.IsValid)
            {
                _appService.SoftDelete(item.Id, this.User.Identity.Name);
            }
            
            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        public IActionResult GetCategorys([DataSourceRequest]DataSourceRequest request)
        {
            DataSourceResult result = _categorysAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).ToDataSourceResult(request);

            return Json(result);
        }
    }
}