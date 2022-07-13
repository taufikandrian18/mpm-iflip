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
    public class AccesoriesCatalogController : FLPControllerBase
    {
        private readonly AccesoriesCatalogAppService _appService;
        private readonly SalesTalkAttachmentAppService _attachmentAppService;

        public AccesoriesCatalogController(AccesoriesCatalogAppService appService, SalesTalkAttachmentAppService attachmentAppService)
        {
            _appService = appService;
            _attachmentAppService = attachmentAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            AccesoriesCatalogs model = new AccesoriesCatalogs();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AccesoriesCatalogs model, string submit, IEnumerable<IFormFile> files)
        {
            if (model != null)
            {
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
                        model.FeaturedImageUrl = await azureController.InsertAndGetUrlAzure(file, model.Id.ToString(), "IMG", "accesoriescatalog");
                    }
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
        public async Task<IActionResult> Edit(AccesoriesCatalogs model, string submit, IEnumerable<IFormFile> files)
        {
            if (model != null)
            {
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.Now;

                if (files.Count() > 0)
                {
                    AzureController azureController = new AzureController();
                    foreach (var file in files)
                    {
                        model.FeaturedImageUrl = await azureController.InsertAndGetUrlAzure(file, model.Id.ToString(), "IMG", "accesoriescatalog");
                    }
                }
                _appService.Update(model);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Grid_Read([DataSourceRequest]DataSourceRequest request)
        {

            DataSourceResult result = _appService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).ToDataSourceResult(request);

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