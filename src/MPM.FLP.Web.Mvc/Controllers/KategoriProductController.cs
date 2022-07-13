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
    public class KategoriProductController : FLPControllerBase
    {
        private readonly ProductCategoryAppService _appService;

        public KategoriProductController(ProductCategoryAppService appService)
        {
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
        //    ProductCategories model = new ProductCategories();
        //    return View(model);
        //}
        public IActionResult Create(ProductCategories model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCategories model, string submit, IEnumerable<IFormFile> files)
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
                model.IconUrl = "";
                if (files.Count() > 0)
                {
                    AzureController azureController = new AzureController();
                    //model.IconUrl = await InsertToAzure(files.FirstOrDefault(), model);
                    model.IconUrl = await azureController.InsertAndGetUrlAzure(files.FirstOrDefault(), model.Id.ToString(), "IMG", "kategoriproduct");
                }

                _appService.Create(model);
                
            };

            return RedirectToAction("Index");
        }

        private async Task<string> InsertToAzure(IFormFile file, ProductCategories model)
        {
            string url = "";
            //var configuration = AppConfigurations.Get(AppDomain.CurrentDomain.BaseDirectory);
            var configuration = new AzureController().GetConnectionToAzure();

            string conn = configuration.GetConnectionString(FLPConsts.AzureConnectionString);

            CloudStorageAccount cloudStorage;
            if (CloudStorageAccount.TryParse(conn, out cloudStorage))
            {
                CloudBlobClient cloudBlobClient = cloudStorage.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("kategoriproduct");

                var path = Path.GetExtension(file.FileName);
                string namaFile = "IMG_" + model.Id + "_" + DateTime.Now.ToString("yyyyMMdd") + path;

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(namaFile);

                //await cloudBlockBlob.UploadFromFileAsync(file.FileName);
                using (Stream stream = file.OpenReadStream())
                {
                    stream.Position = 0;
                    await cloudBlockBlob.UploadFromStreamAsync(stream);
                }

                url = cloudBlockBlob.Uri.AbsoluteUri;
            }

            return url;
        }

        public IActionResult Edit(Guid id)
        {
            var item = _appService.GetById(id);

            return View(item);
            //return View();
        }


        [HttpPost]
        public async Task<IActionResult> Edit(ProductCategories model, string submit, IEnumerable<IFormFile> files)
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
                if (files.Count() > 0)
                {
                    model.IconUrl = await InsertToAzure(files.FirstOrDefault(), model);
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