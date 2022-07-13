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
    public class ClubCategoriesController : FLPControllerBase
    {
        private readonly ClubCommunityCategoryAppService _appService;

        public ClubCategoriesController(ClubCommunityCategoryAppService appService)
        {
            _appService = appService;
        }

        public IActionResult Index()
        {
            TempData["alert"] = "";
            TempData["success"] = "";
            return View();
        }

        //public IActionResult Create()
        //{
        //    ProductCategories model = new ProductCategories();
        //    return View(model);
        //}
        public IActionResult Create(ClubCommunityCategories model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ClubCommunityCategories model, string submit, IEnumerable<IFormFile> images)
        {
            if (model != null)
            {
                if (model.Name == null)
                {
                    TempData["alert"] = "Nama masih kosong";
                    TempData["success"] = "";
                    return RedirectToAction("Create", model);
                }
                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.Now;
                model.CreatorUsername = this.User.Identity.Name;
                model.LastModifierUsername = this.User.Identity.Name;
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

            return RedirectToAction("Index");
        }

       
        public IActionResult Edit(Guid id)
        {
            var item = _appService.GetById(id);

            return View(item);
            //return View();
        }


        [HttpPost]
        public async Task<IActionResult> Edit(ClubCommunityCategories model, string submit, IEnumerable<IFormFile> images)
        {
            if (model != null)
            {
                if (model.Name == null)
                {
                    TempData["alert"] = "Nama masih kosong";
                    TempData["success"] = "";
                    return RedirectToAction("Edit", model.Id);
                }
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.Now;

                if (images.Count() > 0)
                {
                    AzureController azureController = new AzureController();
                    //model.IconUrl = await InsertToAzure(files.FirstOrDefault(), model);
                    model.IconUrl = await azureController.InsertAndGetUrlAzure(images.FirstOrDefault(), model.Id.ToString(), "IMG", "clubcategories");
                }

                _appService.Update(model);

                ViewBag.message = "Berhasil menambahkan data";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Grid_Read([DataSourceRequest]DataSourceRequest request)
        {
            DataSourceResult result = _appService.GetAll().Where(x=> string.IsNullOrEmpty( x.DeleterUsername)).ToDataSourceResult(request);

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

    }
}