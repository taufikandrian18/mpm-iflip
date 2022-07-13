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
using System.Web;

namespace MPM.FLP.Web.Mvc.Controllers
{
    [AbpMvcAuthorize]
    public class SalesTalksController : FLPControllerBase
    {
        private readonly SalesTalkAppService _appService;
        private readonly SalesTalkAttachmentAppService _attachmentAppService;
        private readonly IActivityLogAppService _activityLogAppService;

        public SalesTalksController(SalesTalkAppService appService, SalesTalkAttachmentAppService attachmentAppService, IActivityLogAppService activityLogAppService)
        {
            _appService = appService;
            _attachmentAppService = attachmentAppService;
            _activityLogAppService = activityLogAppService;
        }

        public IActionResult Index()
        {
            TempData["alert"] = "";
            TempData["success"] = "";
            return View();
        }

        public IActionResult Create(SalesTalks model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SalesTalks model, string submit, IEnumerable<IFormFile> files, IEnumerable<IFormFile> documents)
        {
            if (model != null)
            {
                if (model.Title == null)
                {
                    TempData["alert"] = "Judul masih kosong";
                    TempData["success"] = "";
                    return RedirectToAction("Create", model);
                }
                if (model.EndDate.CompareTo(model.StartDate) < 0)
                {
                    TempData["alert"] = "Tanggal akhir lebih besar dari tanggal mulai";
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
                    foreach (var file in files)
                    {
                        model.SalesTalkAttachments.Add(await InsertToAzure(file, model));
                    }

                    model.FeaturedImageUrl = model.SalesTalkAttachments.First().StorageUrl;
                }

                if (documents.Count() > 0)
                {
                    foreach (var file in documents)
                    {
                        model.SalesTalkAttachments.Add(await InsertToAzure(file, model));
                    }
                }
                _appService.Create(model);
            };

            return RedirectToAction("Index");
        }

        private async Task<SalesTalkAttachments> InsertToAzure(IFormFile file, SalesTalks model)
        {
            SalesTalkAttachments attachments = new SalesTalkAttachments();

            var configuration = new AzureController().GetConnectionToAzure();

            string conn = configuration.GetConnectionString(FLPConsts.AzureConnectionString);

            CloudStorageAccount cloudStorage;
            if (CloudStorageAccount.TryParse(conn, out cloudStorage))
            {
                CloudBlobClient cloudBlobClient = cloudStorage.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("salestalk");

                string namaFile = "";
                string order = "";
                string fileType = "";

                if (file.ContentType.Contains("image"))
                    fileType = "IMG";
                else if (file.ContentType.Contains("application"))
                    fileType = "DOC";

                var path = Path.GetExtension(file.FileName);
                if (model.SalesTalkAttachments.Count == 0)
                {
                    namaFile = fileType + "_" + model.Id + "_" + DateTime.Now.ToString("yyyyMMdd") + "_1" + path;
                    order = "1";
                }
                else
                {
                    order = (int.Parse(model.SalesTalkAttachments.OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
                    //if (mode == "Create")
                    //{
                        
                    //}
                    //else
                    //{
                    //    order = (int.Parse(_appService.GetAllAttachments(model.Id).OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
                    //}
                   

                    namaFile = fileType + "_" + model.Id + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + order + path;
                }

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(namaFile);

                //file name bikin baru

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
                attachments.SalesTalkId = model.Id;
                attachments.Title = namaFile;
                attachments.Order = order;
                attachments.StorageUrl = cloudBlockBlob.Uri.AbsoluteUri;
                attachments.FileName = file.FileName;
            }

            return attachments;
        }

        public IActionResult Edit(Guid id)
        {
            var item = _appService.GetById(id);
            item.Contents = HttpUtility.HtmlDecode(item.Contents);

            return View(item);
            //return View();
        }


        [HttpPost]
        public async Task<IActionResult> Edit(SalesTalks model, string submit)
        {
            if (model != null)
            {
                if (model.Title == null)
                {
                    TempData["alert"] = "Judul masih kosong";
                    TempData["success"] = "";
                    return RedirectToAction("Edit", model.Id);
                }
                if (model.EndDate.CompareTo(model.StartDate) < 0)
                {
                    TempData["alert"] = "Tanggal akhir lebih besar dari tanggal mulai";
                    TempData["success"] = "";
                    return RedirectToAction("Edit", model.Id);
                }
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.Now;

                if(model.FeaturedImageUrl == null)
                {
                    model.FeaturedImageUrl = "";
                }

                _appService.Update(model);
            }
            return RedirectToAction("Index");
        }

        public IActionResult EditAttachment(Guid id)
        {
            var item = _appService.GetById(id);
            

            return View(item);
            //return View();
        }
        [HttpPost]
        public IActionResult EditAttachment([DataSourceRequest]DataSourceRequest request,string submit, Guid Id, IEnumerable<IFormFile> files, IEnumerable<IFormFile> documents)
        {
            var model = _appService.GetById(Id);

            if (model != null)
            {
                if (files.Count() > 0)
                {
                    foreach (var file in files)
                    {
                        var newFile = InsertToAzure(file, model).Result;

                        _attachmentAppService.Create(newFile);
                    }

                    model = _appService.GetById(Id);
                    model.LastModifierUsername = this.User.Identity.Name;
                    model.LastModificationTime = DateTime.Now;
                    //Check if deleted is featuredimgurl
                    if (string.IsNullOrEmpty(model.FeaturedImageUrl))
                    {
                        model.FeaturedImageUrl = _appService.GetAllAttachments(Id).FirstOrDefault(x => string.IsNullOrEmpty(x.DeleterUsername)).StorageUrl;
                    }
                    _appService.Update(model);
                }
                if (documents.Count() > 0)
                {
                    foreach (var file in documents)
                    {
                        //model.ServiceTalkFlyerAttachments.Add(await InsertToAzure(file, model, "Edit"));
                        var newFile = InsertToAzure(file, model).Result;

                        _attachmentAppService.Create(newFile);
                    }

                }
                ViewBag.result = "Berhasil menambahkan attachment";
            }
            return RedirectToAction("EditAttachment", model.Id);
            //return View(model);
        }

        public async Task<IActionResult> Grid_Read([DataSourceRequest]DataSourceRequest request)
        {

            //DataSourceResult result = _appService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderByDescending(x => x.CreationTime).ToDataSourceResult(request);

            var model = _appService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderByDescending(x => x.CreationTime);
            var activities = await _activityLogAppService.GetContentActivityLogsSummary(null, null, "salestalk");
            foreach (var item in model)
            {
                int viewCount = (activities.FirstOrDefault(x => x.ContentId == item.Id.ToString())?.Count ?? 0);
                item.ViewCount = (long)viewCount;
            }
            DataSourceResult result = model.ToDataSourceResult(request);

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

        public IActionResult GridAttachment_Read([DataSourceRequest]DataSourceRequest request, Guid modelId)
        {
            DataSourceResult result = _appService.GetAllAttachments(modelId).Where(x=> string.IsNullOrEmpty(x.DeleterUsername) && x.Title.Contains("IMG")).OrderBy(x=>x.Title).ToDataSourceResult(request);
            return Json(result);
        }

        public IActionResult GridAttachmentDocument_Read([DataSourceRequest]DataSourceRequest request, Guid modelId)
        {
            DataSourceResult result = _appService.GetAllAttachments(modelId).Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.Title.Contains("DOC")).OrderBy(x => x.Title).ToDataSourceResult(request);
            return Json(result);
        }

        public IActionResult GridAttachment_Destroy([DataSourceRequest]DataSourceRequest request, SalesTalkAttachments item, Guid modelId)
        {
            if (ModelState.IsValid)
            {
                _attachmentAppService.SoftDelete(item.Id, this.User.Identity.Name);

                //Check if deleted is featuredimgurl
                var model = _appService.GetById(modelId);
                var attachmentUrl = item.StorageUrl;
                if (!string.IsNullOrEmpty(model.FeaturedImageUrl))
                {
                    if (model.FeaturedImageUrl == attachmentUrl)
                    {
                        var attachments = model.SalesTalkAttachments.Where(y => string.IsNullOrEmpty(y.DeleterUsername));

                        if (attachments.Count() > 0 && attachments.Select(x=>x.Title).Any(x=>x.Contains("IMG")))
                        {
                            model.FeaturedImageUrl = attachments.FirstOrDefault(x => x.Title.Contains("IMG") && string.IsNullOrEmpty(x.DeleterUsername)).StorageUrl;
                        }
                        else
                        {
                            model.FeaturedImageUrl = "";
                        }
                        _appService.Update(model);
                    }
                }
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

    }
}