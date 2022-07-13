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
    public class InfoMainDealersController : FLPControllerBase
    {
        private readonly UserManager _userManager;
        private readonly InfoMainDealerAppService _appService;
        private readonly InfoMainDealerAttachmentAppService _attachmentAppService;
        private readonly IActivityLogAppService _activityLogAppService;

        public InfoMainDealersController(UserManager userManager, InfoMainDealerAppService appService, InfoMainDealerAttachmentAppService attachmentAppService, IActivityLogAppService activityLogAppService)
        {
            _userManager = userManager;
            _appService = appService;
            _attachmentAppService = attachmentAppService;
            _activityLogAppService = activityLogAppService;
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
        public IActionResult Create(InfoMainDealers model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(InfoMainDealers model, string submit, IEnumerable<IFormFile> files, IEnumerable<IFormFile> images, IEnumerable<IFormFile> videos)
        {
            if (model != null)
            {
                if(model.Title == null)
                {
                    TempData["alert"] = "Judul masih kosong";
                    TempData["success"] = "";
                    return RedirectToAction("Create",model);
                }
                if (images.Count() > 10)
                {
                    TempData["alert"] = "Gambar yang dimasukkan tidak dapat lebih dari 10";
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

                foreach (var image in images)
                {
                    model.InfoMainDealerAttachments.Add(await InsertToAzure(image, model, "Create"));
                    model.FeaturedImageUrl = model.InfoMainDealerAttachments.FirstOrDefault(x => x.Title.Contains("IMG") && string.IsNullOrEmpty(x.DeleterUsername)).StorageUrl;
                }
                foreach (var file in files)
                {
                    model.InfoMainDealerAttachments.Add(await InsertToAzure(file, model, "Create"));
                }
                foreach (var video in videos)
                {
                    model.InfoMainDealerAttachments.Add(await InsertToAzure(video, model, "Create"));
                }

                _appService.Create(model);
                ViewBag.result = "Data berhasil ditambahkan";

            };

            return RedirectToAction("Index");
        }

        private async Task<InfoMainDealerAttachments> InsertToAzure(IFormFile file, InfoMainDealers model, string mode)
        {
            InfoMainDealerAttachments attachments = new InfoMainDealerAttachments();

            var configuration = new AzureController().GetConnectionToAzure();
            //var configuration = AppConfigurations.Get(AppDomain.CurrentDomain.BaseDirectory);

            string conn = configuration.GetConnectionString(FLPConsts.AzureConnectionString);

            CloudStorageAccount cloudStorage;
            if (CloudStorageAccount.TryParse(conn, out cloudStorage))
            {
                CloudBlobClient cloudBlobClient = cloudStorage.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("infomaindealer");

                string namaFile = "";
                string order = "";
                string fileType = "";

                if (file.ContentType.Contains("image"))
                    fileType = "IMG";
                else if (file.ContentType.Contains("application"))
                    fileType = "DOC";
                else
                    fileType = "VID";

                var path = Path.GetExtension(file.FileName);
                if (model.InfoMainDealerAttachments.Count == 0)
                {
                    namaFile = fileType + "_" + model.Id + "_" + DateTime.Now.ToString("yyyyMMdd") + "_1" + path;
                    order = "1";
                }
                else
                {
                    if (mode == "Create")
                    {
                        order = (int.Parse(model.InfoMainDealerAttachments.OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
                    }
                    else
                    {
                        order = (int.Parse(_appService.GetAllAttachments(model.Id).OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
                    }


                    namaFile = fileType + "_" + model.Id + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + order + path;
                }

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
                attachments.InfoMainDealerId = model.Id;
                attachments.Order = order;
                attachments.Title = namaFile;
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
        public IActionResult Edit(InfoMainDealers model, string submit)
        {
            if (model != null)
            {
                if (model.Title == null)
                {
                    TempData["alert"] = "Judul masih kosong";
                    TempData["success"] = "";
                    return RedirectToAction("Edit", model.Id);
                }
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.Now;

                _appService.Update(model);
                ViewBag.result = "Data berhasil ditambahkan";
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
        public IActionResult UploadAttachment([DataSourceRequest]DataSourceRequest request, string submit, Guid Id, IEnumerable<IFormFile> images, IEnumerable<IFormFile> documents, IEnumerable<IFormFile> videos)
        {
            var model = _appService.GetById(Id);
            IEnumerable<IFormFile> files = images.Count() > 0 ? images : videos.Count() > 0 ? videos : documents;
            TempData["alert"] = "";
            TempData["success"] = "";

            if (model != null)
            {
                if (files.Count() > 0)
                {
                    var tmp = _appService.GetById(Id).InfoMainDealerAttachments.Where(x => x.Title.Contains("IMG") && string.IsNullOrEmpty(x.DeleterUsername)).Count();
                    if (images.Count() > 10)
                    {
                        TempData["alert"] = "Gambar yang dimasukkan tidak dapat lebih dari 10";
                        TempData["success"] = "";
                        return RedirectToAction("EditAttachment", model.Id);
                    }
                    if ((images.Count() + tmp) > 10)
                    {
                        TempData["alert"] = "Jumlah gambar pada storage dan gambar yang akan dimasukkan lebih dari 10 ";
                        TempData["success"] = "";
                        return RedirectToAction("EditAttachment", model.Id);
                    }
                    
                    foreach (var file in files)
                    {
                        //model.GuideAttachments.Add(await InsertToAzure(file, model, "Edit"));
                        var newFile = InsertToAzure(file, model, "Edit").Result;

                        _attachmentAppService.Create(newFile);
                    }

                    model = _appService.GetById(Id);
                    model.LastModifierUsername = this.User.Identity.Name;
                    model.LastModificationTime = DateTime.Now;

                    //Check if featuredimgurl empty
                    if (string.IsNullOrEmpty(model.FeaturedImageUrl) && images.Count() > 0)
                    {
                        model.FeaturedImageUrl = _appService.GetAllAttachments(Id).FirstOrDefault(x => x.Title.Contains("IMG") && string.IsNullOrEmpty(x.DeleterUsername)).StorageUrl;
                    }
                    _appService.Update(model);
                    TempData["alert"] = "";
                    TempData["success"] = "Berhasil menambahkan attachment";
                }
                
            }
            //return View(model);
            //return RedirectToAction("EditAttachment", model.Id);
            return Json(new { success = true });
        }
        public async Task<IActionResult> Grid_Read([DataSourceRequest]DataSourceRequest request)
        {
            //var item = GetGuide();
            //DataSourceResult result = item.ToDataSourceResult(request);
            
            //DataSourceResult result = _appService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderByDescending(x => x.CreationTime).ToDataSourceResult(request);

            var model = _appService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderByDescending(x => x.CreationTime);
            var activities = await _activityLogAppService.GetContentActivityLogsSummary(null, null, "infomaindealer");
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

        public IActionResult GridAttachmentImage_Read([DataSourceRequest]DataSourceRequest request, Guides model)
        {
            DataSourceResult result = _appService.GetAllAttachments(model.Id).Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.Title.Contains("IMG")).OrderBy(x => x.Title).ToDataSourceResult(request);
            return Json(result);
        }
        public IActionResult GridAttachmentDocument_Read([DataSourceRequest]DataSourceRequest request, Guides model)
        {
            DataSourceResult result = _appService.GetAllAttachments(model.Id).Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.Title.Contains("DOC")).OrderBy(x => x.Title).ToDataSourceResult(request);
            return Json(result);
        }
        public IActionResult GridAttachmentVideo_Read([DataSourceRequest]DataSourceRequest request, Guides model)
        {
            var t = _appService.GetAllAttachments(model.Id).ToList();
            DataSourceResult result = _appService.GetAllAttachments(model.Id).Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.Title.Contains("VID")).OrderBy(x => x.Title).ToDataSourceResult(request);
            
            return Json(result);
        }

        public IActionResult GridAttachment_Destroy([DataSourceRequest]DataSourceRequest request, InfoMainDealerAttachments item, Guid modelId)
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
                        var attachments = model.InfoMainDealerAttachments.Where(y => string.IsNullOrEmpty(y.DeleterUsername));

                        if (attachments.Count() > 0 && attachments.Select(x=>x.Title).Any(x=>x.Contains("IMG")))
                        {
                            model.FeaturedImageUrl = attachments.FirstOrDefault(x => x.Title.Contains("IMG"))?.StorageUrl;
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