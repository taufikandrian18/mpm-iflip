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
    public class ManagementPanduanTeknikalController : FLPControllerBase
    {
        private readonly UserManager _userManager;
        private readonly GuideAppService _appService;
        private readonly GuideCategoryAppService _categoryAppService;
        private readonly GuideAttachmentAppService _attachmentAppService;
        private readonly IActivityLogAppService _activityLogAppService;

        public ManagementPanduanTeknikalController(UserManager userManager, GuideAppService appService, GuideCategoryAppService categoryAppService, GuideAttachmentAppService attachmentAppService, IActivityLogAppService activityLogAppService)
        {
            _userManager = userManager;
            _appService = appService;
            _categoryAppService = categoryAppService;
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
        public IActionResult Create(Guides model)
        {
            return View(model);
        }

        public IActionResult GetGuideCategories([DataSourceRequest]DataSourceRequest request)
        {
            IEnumerable<GuideCategories> guideCategories = _categoryAppService.GetAll();

            DataSourceResult result = guideCategories.ToDataSourceResult(request);

            return Json(result);
        }


        [HttpPost]
        public async Task<IActionResult> Create(Guides model, string submit, IEnumerable<IFormFile> files, IEnumerable<IFormFile> images, IEnumerable<IFormFile> videos)
        {
            if (model != null)
            {
                if(model.Title == null)
                {
                    TempData["alert"] = "Judul masih kosong";
                    TempData["success"] = "";
                    return RedirectToAction("Create",model);
                }
                var user = _userManager.Users.FirstOrDefault(x => x.Id == this.User.Identity.GetUserId());
                var roles = _userManager.GetRolesAsync(user).Result.ToList();

                string resource = null;
                if (roles.FirstOrDefault().Contains("H1"))
                {
                    resource = "H1";
                }
                else if (roles.FirstOrDefault().Contains("H2"))
                {
                    resource = "H2";
                }
                else if (roles.FirstOrDefault().Contains("H3"))
                {
                    resource = "H3";
                }
                else if (roles.FirstOrDefault().Contains("HC3"))
                {
                    resource = "HC3";
                }
                else {
                    resource = "";
                }

                model.Id = Guid.NewGuid();
                model.IsTechnicalGuide = true;
                model.CreationTime = DateTime.Now;
                model.CreatorUsername = this.User.Identity.Name;
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.Now;
                model.DeleterUsername = "";
                model.FeaturedImageUrl = "";
                model.Resource = resource;

                foreach (var image in images)
                {
                    model.GuideAttachments.Add(await InsertToAzure(image, model, "Create"));
                    model.FeaturedImageUrl = model.GuideAttachments.FirstOrDefault(x => x.Title.Contains("IMG") && string.IsNullOrEmpty(x.DeleterUsername)).StorageUrl;
                }
                foreach (var file in files)
                {
                    model.GuideAttachments.Add(await InsertToAzure(file, model, "Create"));
                }
                foreach (var video in videos)
                {
                    model.GuideAttachments.Add(await InsertToAzure(video, model, "Create"));
                }

                _appService.Create(model, "Panduan Teknikal");
                ViewBag.result = "Data berhasil ditambahkan";

            };

            return RedirectToAction("Index");
        }

        private async Task<GuideAttachments> InsertToAzure(IFormFile file, Guides model, string mode)
        {
            GuideAttachments attachments = new GuideAttachments();

            var configuration = new AzureController().GetConnectionToAzure();
            //var configuration = AppConfigurations.Get(AppDomain.CurrentDomain.BaseDirectory);

            string conn = configuration.GetConnectionString(FLPConsts.AzureConnectionString);

            CloudStorageAccount cloudStorage;
            if (CloudStorageAccount.TryParse(conn, out cloudStorage))
            {
                CloudBlobClient cloudBlobClient = cloudStorage.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("panduanteknikal");

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
                if (model.GuideAttachments.Count == 0)
                {
                    namaFile = fileType + "_" + model.Id + "_" + DateTime.Now.ToString("yyyyMMdd") + "_1" + path;
                    order = "1";
                }
                else
                {
                    if (mode == "Create")
                    {
                        order = (int.Parse(model.GuideAttachments.OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
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
                attachments.GuideId = model.Id;
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
        public IActionResult Edit(Guides model, string submit)
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

                _appService.Update(model, "Panduan Teknikal");
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
                    _appService.Update(model, "Panduan Teknikal");
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
            var user = _userManager.Users.FirstOrDefault(x => x.Id == this.User.Identity.GetUserId());
            var roles = _userManager.GetRolesAsync(user).Result.ToList();

            string resource = null;
            if (roles.FirstOrDefault().Contains("H1"))
            {
                resource = "H1";
            }
            else if (roles.FirstOrDefault().Contains("H2"))
            {
                resource = "H2";
            }
            else if (roles.FirstOrDefault().Contains("H3"))
            {
                resource = "H3";
            }
            else if (roles.FirstOrDefault().Contains("HC3"))
            {
                resource = "HC3";
            }

            //DataSourceResult result = _appService.GetAll().Where(x => x.IsTechnicalGuide == true && (x.Resource == resource || resource == null) && string.IsNullOrEmpty(x.DeleterUsername)).OrderByDescending(x => x.CreationTime).ToDataSourceResult(request);

            var model = _appService.GetAll().Where(x => x.IsTechnicalGuide == true && (x.Resource == resource || resource == null) && string.IsNullOrEmpty(x.DeleterUsername)).OrderByDescending(x => x.CreationTime);
            var activities = await _activityLogAppService.GetContentActivityLogsSummary(null, null, "guide");
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
                _appService.SoftDelete(item.Id, this.User.Identity.Name, "Panduan Teknikal");
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

        public IActionResult GridAttachment_Destroy([DataSourceRequest]DataSourceRequest request, GuideAttachments item, Guid modelId)
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
                        var attachments = model.GuideAttachments.Where(y => string.IsNullOrEmpty(y.DeleterUsername));

                        if (attachments.Count() > 0 && attachments.Select(x=>x.Title).Any(x=>x.Contains("IMG")))
                        {
                            model.FeaturedImageUrl = attachments.FirstOrDefault(x => x.Title.Contains("IMG") && string.IsNullOrEmpty(x.DeleterUsername)).StorageUrl;
                        }
                        else
                        {
                            model.FeaturedImageUrl = "";
                        }
                        _appService.Update(model, "Panduan Teknikal");
                    }
                }
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }
    }
}