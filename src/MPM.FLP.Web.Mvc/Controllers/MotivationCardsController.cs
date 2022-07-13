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
using MPM.FLP.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using MPM.FLP.Authorization.Users;
using Abp.Runtime.Security;

namespace MPM.FLP.Web.Mvc.Controllers
{
    [AbpMvcAuthorize]
    public class MotivationCardsController : FLPControllerBase
    {
        private readonly UserManager _userManager;
        private readonly MotivationCardAppService _appService;

        public MotivationCardsController(UserManager userManager, MotivationCardAppService appService)
        {
            _userManager = userManager;
            _appService = appService;
        }

        public IActionResult BackToIndex()
        {
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            TempData["alert"] = "";
            TempData["success"] = "";
            return View();
        }
        //public IActionResult Create()
        //{
        //    Guides model = new Guides();

        //    return View(model);
        //}
        public IActionResult Create(MotivationCards model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MotivationCards model, string submit, IEnumerable<IFormFile> files, IEnumerable<IFormFile> images)
        {
            if (model != null)
            {
                if(model.Title == null)
                {
                    TempData["alert"] = "Judul masih kosong";
                    TempData["success"] = "";
                    return RedirectToAction("Create",model);
                }

                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.Now;
                model.CreatorUsername = this.User.Identity.Name;
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.Now;
                model.DeleterUsername = "";
                model.StorageUrl = "";

                foreach (var image in images)
                {
                    AzureController azureController = new AzureController();
                    model.StorageUrl = await azureController.InsertAndGetUrlAzure(image, model.Id.ToString(), "IMG", "motivationcards");
                }

                _appService.Create(model);
                ViewBag.result = "Data berhasil ditambahkan";

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
        public async Task<IActionResult> Edit(MotivationCards model, string submit, IEnumerable<IFormFile> images)
        {
            if (model != null)
            {
                if (model.Title == null)
                {
                    TempData["alert"] = "Judul masih kosong";
                    TempData["success"] = "";
                    return RedirectToAction("Edit", model.Id);
                }
                if(images.Count() > 0)
                {
                    AzureController azureController = new AzureController();
                    model.StorageUrl = await azureController.InsertAndGetUrlAzure(images.FirstOrDefault(), model.Id.ToString(), "IMG", "motivationcards");
                }
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.Now;

                _appService.Update(model);
                ViewBag.result = "Data berhasil ditambahkan";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Grid_Read([DataSourceRequest]DataSourceRequest request)
        {
            //var item = GetGuide();
            //DataSourceResult result = item.ToDataSourceResult(request);
            
            DataSourceResult result = _appService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderByDescending(x => x.CreationTime).ToDataSourceResult(request);
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