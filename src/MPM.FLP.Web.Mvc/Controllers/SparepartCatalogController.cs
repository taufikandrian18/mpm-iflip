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
using System.Net;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace MPM.FLP.Web.Mvc.Controllers
{
    [AbpMvcAuthorize]
    public class SparepartCatalogController : FLPControllerBase
    {
        private readonly ProductCatalogAppService _appService;
        private readonly IHostingEnvironment _hostingEnvironment;


        public SparepartCatalogController(ProductCatalogAppService appService, IHostingEnvironment hostingEnvironment)
        {
            _appService = appService;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Main View
        


        private async Task<ProductCatalogAttachments> InsertToAzureCreate(IFormFile file, ProductCatalogs model, string nama)
        {
            ProductCatalogAttachments attachments = new ProductCatalogAttachments();

            var configuration = new AzureController().GetConnectionToAzure();
            //var configuration = AppConfigurations.Get(AppDomain.CurrentDomain.BaseDirectory);

            string conn = configuration.GetConnectionString(FLPConsts.AzureConnectionString);

            CloudStorageAccount cloudStorage;
            if (CloudStorageAccount.TryParse(conn, out cloudStorage))
            {
                CloudBlobClient cloudBlobClient = cloudStorage.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("productcatalog");

                var path = Path.GetExtension(file.FileName);

                string namaFile = nama + "_" + model.Id + "_" + DateTime.Now.ToString("yyyyMMdd") + path;

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(namaFile);

                //await cloudBlockBlob.UploadFromFileAsync(file.FileName);
                using (Stream stream = file.OpenReadStream())
                {
                    stream.Position = 0;
                    await cloudBlockBlob.UploadFromStreamAsync(stream);
                }

                attachments.Id = Guid.NewGuid();
                attachments.CreationTime = DateTime.Now;
                attachments.CreatorUsername = this.User.Identity.Name;
                attachments.LastModifierUsername = this.User.Identity.Name;
                attachments.DeleterUsername = null;
                attachments.ProductCatalogId = model.Id;
                attachments.Order = "";
                attachments.DeleterUsername = "";
                attachments.Title = namaFile;
                attachments.StorageUrl = cloudBlockBlob.Uri.AbsoluteUri;
                attachments.FileName = file.FileName;
            }

            return attachments;
        }

        public IActionResult Edit(Guid id)
        {
            var item = _appService.GetById(id);

            return View(item);
            //return View();
        }

        public IActionResult PartialUpload(Guid id)
        {
            ProductCatalogs model = new ProductCatalogs();
            model.Id = id;
            return PartialView("PartialUpload");
        }

        public IActionResult DownloadFile(Guid id)
        {
            var model = _appService.GetById(id);

            byte[] fileData = null;

            if (model != null)
            {
                using (var client = new WebClient())
                {
                    if (!string.IsNullOrEmpty(model.SparepartDocUrl)){
                        fileData = client.DownloadData(model.SparepartDocUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return File(new MemoryStream(fileData), "application/pdf", "DownloadName.pdf");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadDocument(Guid id,IEnumerable<IFormFile> files)
        {
            AzureController azureController = new AzureController();
            ProductCatalogs model = _appService.GetById(id);
            if(files.Count() > 0)
            {
                model.SparepartDocUrl = await azureController.InsertAndGetUrlAzure(files.First(), id.ToString(), "DOC", "sparepartcatalog");
                _appService.Update(model);
            }
            
            return RedirectToAction("Index");
            //return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductCatalogs model, string submit, IEnumerable<IFormFile> files)
        {
            if (model != null)
            {
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.Now;

                foreach (var file in files)
                {
                    model.ProductCatalogAttachments.Add(await InsertToAzureCreate(file, model, "IMG"));
                }
                
                if(model.ProductCatalogAttachments.Count > 0)
                    model.FeaturedImageUrl = model.ProductCatalogAttachments.Where(x => x.StorageUrl.Contains("IMG")).FirstOrDefault().StorageUrl;

                _appService.Update(model);

                ViewBag.message = "Berhasil mengubah data";
            }
            //RedirectToAction("Index");
            return Redirect("~/ProductCatalog/Index");
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

        #endregion

       
    }
}