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
using MPM.FLP.Authorization.Users;
using Abp.Runtime.Security;

namespace MPM.FLP.Web.Mvc.Controllers
{
    [AbpMvcAuthorize]
    public class ProgramManagementController : FLPControllerBase
    {
        private readonly UserManager _userManager;
        private readonly SalesProgramAppService _appService;
        private readonly SalesProgramAttachmentAppService _attachmentAppService;
        private readonly IActivityLogAppService _activityLogAppService;

        public ProgramManagementController(UserManager userManager, SalesProgramAppService appService, SalesProgramAttachmentAppService attachmentAppService, IActivityLogAppService activityLogAppService)
        {
            _userManager = userManager;
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
        //    Programs model = new Programs();
        //    return View(model);
        //}
        public IActionResult Create(SalesPrograms model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SalesPrograms model, string submit, IEnumerable<IFormFile> files, IEnumerable<IFormFile> images, IEnumerable<IFormFile> videos)
        {
            if (ModelState.IsValid)
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
                    model.SalesProgramAttachments.Add(await InsertToAzure(image, model, "Create"));
                    model.FeaturedImageUrl = model.SalesProgramAttachments.Where(x => x.Title.Contains("IMG")).FirstOrDefault().StorageUrl;
                }
                foreach (var file in files)
                {
                    model.SalesProgramAttachments.Add(await InsertToAzure(file, model, "Create"));
                }
                foreach (var video in videos)
                {
                    model.SalesProgramAttachments.Add(await InsertToAzure(video, model, "Create"));
                }

                _appService.Create(model);
                return RedirectToAction("Index");
            };

            return View(model);
        }

        private async Task<SalesProgramAttachments> InsertToAzure(IFormFile file, SalesPrograms model, string mode)
        {
            SalesProgramAttachments attachments = new SalesProgramAttachments();
            

            //Local
            //var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            //Publish
            //var configuration = AppConfigurations.Get(AppDomain.CurrentDomain.BaseDirectory);

            var configuration = new AzureController().GetConnectionToAzure();

            string conn = configuration.GetConnectionString(FLPConsts.AzureConnectionString);

            CloudStorageAccount cloudStorage;
            if (CloudStorageAccount.TryParse(conn, out cloudStorage))
            {
                CloudBlobClient cloudBlobClient = cloudStorage.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("programsmanagement");

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

                if (model.SalesProgramAttachments.Where(x=>string.IsNullOrEmpty(x.DeleterUsername)).Count() == 0 )
                {
                    namaFile = fileType + "_" + model.Id + "_" + DateTime.Now.ToString("yyyyMMdd") + "_1" + path;
                    order = "1";
                }
                else
                {
                    if (mode == "Create")
                    {
                        order = (int.Parse(model.SalesProgramAttachments.Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
                    }
                    else
                    {
                        order = (int.Parse(_appService.GetAllAttachments(model.Id).Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
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
                attachments.SalesProgramId = model.Id;
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
        public async Task<IActionResult> Edit(SalesPrograms model, string submit)
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
        public IActionResult UploadAttachment([DataSourceRequest]DataSourceRequest request, string submit, Guid Id, IEnumerable<IFormFile> images, IEnumerable<IFormFile> videos, IEnumerable<IFormFile> documents)
        {
            var model = _appService.GetById(Id);
            IEnumerable<IFormFile> files = images.Count() > 0 ? images : videos.Count() > 0 ? videos : documents;

            if (model != null)
            {
                if (files.Count() > 0)
                {
                    var tmp = _appService.GetById(Id).SalesProgramAttachments.Where(x => x.Title.Contains("IMG") && string.IsNullOrEmpty(x.DeleterUsername)).Count();
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
                        //model.ProgramAttachments.Add(await InsertToAzure(file, model, "Edit"));

                        var newFile = InsertToAzure(file, model, "Edit").Result;

                        _attachmentAppService.Create(newFile);
                    }

                    model = _appService.GetById(Id);
                    //Check if featuredimgurl is empty
                    if (string.IsNullOrEmpty(model.FeaturedImageUrl) && images.Count() > 0)
                    {
                        model.FeaturedImageUrl = _appService.GetAllAttachments(Id).FirstOrDefault(x => x.Title.Contains("IMG") && string.IsNullOrEmpty(x.DeleterUsername)).StorageUrl;
                    }
                    model.LastModifierUsername = this.User.Identity.Name;
                    model.LastModificationTime = DateTime.Now;

                    _appService.Update(model);
                    TempData["alert"] = "";
                    TempData["success"] = "Berhasil menambahkan attachment";
                    ViewBag.result = "Berhasil menambahkan attachment";
                }
               
            }
            //return View(model);
            //return RedirectToAction("EditAttachment", model.Id);
            return Json(new { success = true });
        }

        public async Task<IActionResult> Grid_Read([DataSourceRequest]DataSourceRequest request)
        {
            //var user = _userManager.Users.FirstOrDefault(x => x.Id == this.User.Identity.GetUserId());
            //var roles = _userManager.GetRolesAsync(user).Result.ToList();

            //string resource = null;
            //if (roles.FirstOrDefault().Contains("H1"))
            //{
            //    resource = "H1";
            //}
            //else if (roles.FirstOrDefault().Contains("H2"))
            //{
            //    resource = "H2";
            //}
            //else if (roles.FirstOrDefault().Contains("H3"))
            //{
            //    resource = "H3";
            //}
            //else if (roles.FirstOrDefault().Contains("HC3"))
            //{
            //    resource = "HC3";
            //}

            //DataSourceResult result = _appService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).ToDataSourceResult(request);

            var model = _appService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderByDescending(x => x.CreationTime);
            var activities = await _activityLogAppService.GetContentActivityLogsSummary(null, null, "salesprogram");
            foreach (var item in model)
            {
                int viewCount = (activities.FirstOrDefault(x => x.ContentId == item.Id.ToString())?.Count ?? 0);
                item.ViewCount = (long)viewCount;
            }
            DataSourceResult result = model.ToDataSourceResult(request);

            return Json(result);
        }

        public IActionResult Grid_Destroy([DataSourceRequest]DataSourceRequest request, Programs item)
        {
            if (ModelState.IsValid)
            {
                _appService.SoftDelete(item.Id, this.User.Identity.Name);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        public IActionResult GridAttachmentImage_Read([DataSourceRequest]DataSourceRequest request, Guid modelId)
        {
            DataSourceResult result = _appService.GetAllAttachments(modelId).Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.Title.Contains("IMG")).OrderBy(x => x.Title).ToDataSourceResult(request);
            return Json(result);
        }
        public IActionResult GridAttachmentDocument_Read([DataSourceRequest]DataSourceRequest request, Guid modelId)
        {
            DataSourceResult result = _appService.GetAllAttachments(modelId).Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.Title.Contains("DOC")).OrderBy(x => x.Title).ToDataSourceResult(request);
            return Json(result);
        }
        public IActionResult GridAttachmentVideo_Read([DataSourceRequest]DataSourceRequest request, Guid modelId)
        {
            DataSourceResult result = _appService.GetAllAttachments(modelId).Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.Title.Contains("VID")).OrderBy(x => x.Title).ToDataSourceResult(request);
            return Json(result);
        }

        public IActionResult GridAttachment_Destroy([DataSourceRequest]DataSourceRequest request, Guid modelId, SalesProgramAttachments item)
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
                        var attachments = model.SalesProgramAttachments.Where(y => string.IsNullOrEmpty(y.DeleterUsername));

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