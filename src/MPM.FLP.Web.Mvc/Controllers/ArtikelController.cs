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
using System.Threading.Tasks;
using MPM.FLP.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using MPM.FLP.Authorization.Users;
using Abp.Runtime.Security;
using MPM.FLP.Authorization.Roles;
using Abp.Runtime.Session;
using System.Web;

namespace MPM.FLP.Web.Mvc.Controllers
{
    [AbpMvcAuthorize]
    public class ArtikelController : FLPControllerBase
    {
        private readonly UserManager _userManager;
        private readonly ArticleAppService _appService;
        private readonly ArticleAttachmentAppService _attachmentAppService;
        private readonly IActivityLogAppService _activityLogAppService;

        public ArtikelController(ArticleAppService appService, ArticleAttachmentAppService attachmentAppService, UserManager userManager,  RoleManager roleManager, IActivityLogAppService activityLogAppService)
        {
            _appService = appService;
            _attachmentAppService = attachmentAppService;
            _userManager = userManager;
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
        //    Articles model = new Articles();
        //    return View(model);
        //}
        public IActionResult Create(Articles model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Articles model, string submit, IEnumerable<IFormFile> files, IEnumerable<IFormFile> images, IEnumerable<IFormFile> videos)
        {
            if (model != null)
            {
                if (model.Title == null)
                {
                    TempData["alert"] = "Judul masih kosong";
                    TempData["success"] = "";
                    return RedirectToAction("Create", model);
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

                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.Now;
                model.CreatorUsername = this.User.Identity.Name;
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.Now;
                model.DeleterUsername = "";
                model.ViewCount = 0;
                model.Resource = resource;
                

                if (images.Count() > 0)
                {
                    foreach (var image in images)
                    {
                        model.ArticleAttachments.Add(await InsertToAzure(image, model, "Create"));
                    }

                    model.FeaturedImageUrl = model.ArticleAttachments.First().StorageUrl;
                }

                foreach (var file in files)
                {
                    if (file.ContentType.Contains("application"))
                        model.ArticleAttachments.Add(await InsertToAzure(file, model, "Create"));
                }
                foreach (var video in videos)
                {
                    model.ArticleAttachments.Add(await InsertToAzure(video, model, "Create"));
                }

                _appService.Create(model);
            }
            return RedirectToAction("Index");
        }

        private async Task<ArticleAttachments> InsertToAzure(IFormFile file, Articles model, string mode)
        {
            ArticleAttachments attachments = new ArticleAttachments();

            //var configuration = AppConfigurations.Get(AppDomain.CurrentDomain.BaseDirectory);
            var configuration = new AzureController().GetConnectionToAzure();
            //IConfigurationRoot configuration = new ConfigurationBuilder()
            //                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            //                .AddJsonFile("appsettings.json")
            //                .Build();

            string conn = configuration.GetConnectionString(FLPConsts.AzureConnectionString);

            CloudStorageAccount cloudStorage;
            if (CloudStorageAccount.TryParse(conn, out cloudStorage))
            {
                CloudBlobClient cloudBlobClient = cloudStorage.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("article");
                

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
                if (model.ArticleAttachments.Count == 0)
                {
                    namaFile = fileType+"_" + model.Id + "_" + DateTime.Now.ToString("yyyyMMdd") + "_1" + path;
                    order = "1";
                }
                else
                {
                    if (mode == "Create")
                    {
                        order = (int.Parse(model.ArticleAttachments.OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
                    }
                    else
                    {
                        order = (int.Parse(_appService.GetAllAttachments(model.Id).OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
                    }


                    namaFile = fileType+"_" + model.Id + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + order + path;
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
                attachments.ArticleId = model.Id;
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
        }

        [HttpPost]
        public IActionResult Edit(Articles model, string submit)
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
        public IActionResult EditAttachment([DataSourceRequest]DataSourceRequest request, string submit, Guid Id, IEnumerable<IFormFile> files, IEnumerable<IFormFile> documents, IEnumerable<IFormFile> videos)
        {
            var model = _appService.GetById(Id);
            IEnumerable<IFormFile> filers = files.Count() > 0 ? files : videos.Count() > 0 ? videos : documents;
            if (model != null)
            {
                if (filers.Count() > 0)
                {
                    foreach (var file in filers)
                    {
                        //model.ArticleAttachments.Add(await InsertToAzure(file, model, "Edit"));
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
                    TempData["alert"] = "";
                    TempData["success"] = "Berhasil menambahkan attachment";
                }
                else
                {
                    TempData["alert"] = "";
                    TempData["success"] = "";
                }
                
            }
            //return View(model);
            return RedirectToAction("EditAttachment", model.Id);
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        public async Task<IActionResult> Grid_Read([DataSourceRequest]DataSourceRequest request)
        {
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

            var articles = _appService.GetAll().Where(x => (x.Resource == resource || resource == null) && string.IsNullOrEmpty(x.DeleterUsername)).OrderByDescending(x => x.CreationTime).ToList();
            var activities = await _activityLogAppService.GetContentActivityLogsSummary(null, null, "article");
            foreach (var article in articles)
            {
                int viewCount = (activities.FirstOrDefault(x => x.ContentId == article.Id.ToString())?.Count ?? 0);
                article.ViewCount = (long)viewCount;
            }
            DataSourceResult result = articles.ToDataSourceResult(request);

            return Json(result);
        }

        public IActionResult Grid_Destroy([DataSourceRequest]DataSourceRequest request, Articles item)
        {
            if (ModelState.IsValid)
            {
                _appService.SoftDelete(item.Id,this.User.Identity.Name);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        public IActionResult GridAttachment_Read([DataSourceRequest]DataSourceRequest request, Guid modelId)
        {
            DataSourceResult result = _appService.GetAllAttachments(modelId).Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.Title.Contains("IMG")).OrderBy(x => x.Title).ToDataSourceResult(request);
            return Json(result);
        }

        public IActionResult GridAttachmentDocument_Read([DataSourceRequest] DataSourceRequest request, Guid modelId)
        {
            DataSourceResult result = _appService.GetAllAttachments(modelId).Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.Title.Contains("DOC")).OrderBy(x => x.Title).ToDataSourceResult(request);
            return Json(result);
        }
        public IActionResult GridAttachmentVideo_Read([DataSourceRequest] DataSourceRequest request, Guid modelId)
        {
            var t = _appService.GetAllAttachments(modelId).ToList();
            DataSourceResult result = _appService.GetAllAttachments(modelId).Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.Title.Contains("VID")).OrderBy(x => x.Title).ToDataSourceResult(request);

            return Json(result);
        }


        public IActionResult GridAttachment_Destroy([DataSourceRequest]DataSourceRequest request, ArticleAttachments item, Guid modelId)
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
                        var attachments = model.ArticleAttachments.Where(y => string.IsNullOrEmpty(y.DeleterUsername));

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