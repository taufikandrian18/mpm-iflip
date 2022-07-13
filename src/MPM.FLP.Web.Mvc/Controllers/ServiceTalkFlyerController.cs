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
using Abp.Runtime.Validation;

namespace MPM.FLP.Web.Mvc.Controllers
{
    [AbpMvcAuthorize]
    public class ServiceTalkFlyerController : FLPControllerBase
    {
        private readonly ServiceTalkFlyerAppService _appService;
        private readonly ServiceTalkFlyerAttachmentAppService _attachmentAppService;
        private readonly IActivityLogAppService _activityLogAppService;

        public ServiceTalkFlyerController(ServiceTalkFlyerAppService appService, ServiceTalkFlyerAttachmentAppService attachmentAppService, IActivityLogAppService activityLogAppService)
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

        //public IActionResult Create()
        //{
        //    ServiceTalkFlyers model = new ServiceTalkFlyers();
        //    return View(model);
        //}
        public IActionResult Create(ServiceTalkFlyers model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ServiceTalkFlyers model, string submit, IEnumerable<IFormFile> files, IEnumerable<IFormFile> documents)
        {
            if (model != null)
            {
                if (model.Title == null)
                {
                    TempData["alert"] = "Judul masih kosong";
                    TempData["success"] = "";
                    return RedirectToAction("Create", model);
                }
                if(model.EndDate.CompareTo(model.StartDate) < 0)
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
                        model.ServiceTalkFlyerAttachments.Add(await InsertToAzure(file, model, "Create"));
                    }

                    model.FeaturedImageUrl = model.ServiceTalkFlyerAttachments.First().StorageUrl;
                }
                if(documents.Count() > 0)
                {
                    foreach (var file in documents)
                    {
                        model.ServiceTalkFlyerAttachments.Add(await InsertToAzure(file, model, "Create"));
                    }
                }
                _appService.Create(model);
            };

            return RedirectToAction("Index");
        }

        private async Task<ServiceTalkFlyerAttachments> InsertToAzure(IFormFile file, ServiceTalkFlyers model, string mode)
        {
            ServiceTalkFlyerAttachments attachments = new ServiceTalkFlyerAttachments();

            var configuration = new AzureController().GetConnectionToAzure();
            //For publish Connection
            //var configuration = AppConfigurations.Get(AppDomain.CurrentDomain.BaseDirectory);

            //For local connection
            //var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            string conn = configuration.GetConnectionString(FLPConsts.AzureConnectionString);

            CloudStorageAccount cloudStorage;
            if (CloudStorageAccount.TryParse(conn, out cloudStorage))
            {
                CloudBlobClient cloudBlobClient = cloudStorage.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("servicetalkflyer");

                string namaFile = "";
                string order = "";
                string fileType = "";

                if (file.ContentType.Contains("image"))
                    fileType = "IMG";
                else if (file.ContentType.Contains("application"))
                    fileType = "DOC";

                var path = Path.GetExtension(file.FileName);

                if (model.ServiceTalkFlyerAttachments.Count == 0)
                {
                    namaFile = fileType + "_" + model.Id + "_" + DateTime.Now.ToString("yyyyMMdd") + "_1" + path;
                    order = "1";
                }
                else
                {
                    if(mode == "Create")
                    {
                        order = (int.Parse(model.ServiceTalkFlyerAttachments.OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
                    }
                    else
                    {
                        order = (int.Parse(_appService.GetAllAttachments(model.Id).OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
                    }
                   

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
                attachments.ServiceTalkFlyerId = model.Id;
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
        public async Task<IActionResult> Edit(ServiceTalkFlyers model, string submit)
        {
            if (model != null)
            {
                if (model.Title == null)
                {
                    TempData["alert"] = "Judul masih kosong";
                    TempData["success"] = "";
                    return RedirectToAction("Edit", model);
                }
                if (model.EndDate.CompareTo(model.StartDate) < 0)
                {
                    TempData["alert"] = "Tanggal akhir lebih besar dari tanggal mulai";
                    TempData["success"] = "";
                    return RedirectToAction("Edit", model);
                }
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.Now;

                if (model.FeaturedImageUrl == null)
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
                        //model.ServiceTalkFlyerAttachments.Add(await InsertToAzure(file, model, "Edit"));
                        var newFile = InsertToAzure(file, model, "Edit").Result;

                        _attachmentAppService.Create(newFile);
                    }

                    model = _appService.GetById(Id);
                    //Check if featuredimgurl empty
                    if (string.IsNullOrEmpty(model.FeaturedImageUrl))
                    {
                        model.FeaturedImageUrl = _appService.GetAllAttachments(Id).FirstOrDefault(x => string.IsNullOrEmpty(x.DeleterUsername)).StorageUrl;
                    }
                    model.LastModifierUsername = this.User.Identity.Name;
                    model.LastModificationTime = DateTime.Now;

                    _appService.Update(model);
                }
                if (documents.Count() > 0)
                {
                    foreach (var file in documents)
                    {
                        //model.ServiceTalkFlyerAttachments.Add(await InsertToAzure(file, model, "Edit"));
                        var newFile = InsertToAzure(file, model, "Edit").Result;

                        _attachmentAppService.Create(newFile);
                    }

                    _appService.Update(model);
                }
                ViewBag.result = "Berhasil menambahkan attachment";
            }
            //return View(model);
            return RedirectToAction("EditAttachment", model.Id);
        }

        public async Task<IActionResult> Grid_Read([DataSourceRequest]DataSourceRequest request)
        {

            //DataSourceResult result = _appService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderByDescending(x => x.CreationTime).ToDataSourceResult(request);

            var model = _appService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderByDescending(x => x.CreationTime);
            var activities = await _activityLogAppService.GetContentActivityLogsSummary(null, null, "servicetalkflyer");
            foreach (var item in model)
            {
                int viewCount = (activities.FirstOrDefault(x => x.ContentId == item.Id.ToString())?.Count ?? 0);
                item.ViewCount = (long)viewCount;
            }
            DataSourceResult result = model.ToDataSourceResult(request);

            return Json(result);
        }
        
        public IActionResult Grid_Destroy([DataSourceRequest]DataSourceRequest request, ServiceTalkFlyers item)
        {
            if (ModelState.IsValid)
            {
                _appService.SoftDelete(item.Id, this.User.Identity.Name);
            }
            
            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        public IActionResult GridAttachment_Read([DataSourceRequest]DataSourceRequest request, ServiceTalkFlyers model)
        {
            DataSourceResult result = _appService.GetAllAttachments(model.Id).Where(x=> string.IsNullOrEmpty(x.DeleterUsername) && x.Title.Contains("IMG")).OrderBy(x=>x.Title).ToDataSourceResult(request);
            return Json(result);
        }

        public IActionResult GridAttachmentDocument_Read([DataSourceRequest]DataSourceRequest request, ServiceTalkFlyers model)
        {
            DataSourceResult result = _appService.GetById(model.Id).ServiceTalkFlyerAttachments.Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.Title.Contains("DOC")).OrderBy(x => x.Title).ToDataSourceResult(request);
            return Json(result);
        }

        public IActionResult GridAttachment_Destroy([DataSourceRequest]DataSourceRequest request, ServiceTalkFlyerAttachments item, Guid modelId)
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
                        var attachments = model.ServiceTalkFlyerAttachments.Where(y => string.IsNullOrEmpty(y.DeleterUsername));

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