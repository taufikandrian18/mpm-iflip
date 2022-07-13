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
    public class BrandCampaignsController : FLPControllerBase
    {
        private readonly BrandCampaignAppService _appService;
        private readonly BrandCampaignAttachmentAppService _attachmentAppService;
        private readonly IActivityLogAppService _activityLogAppService;

        public BrandCampaignsController(BrandCampaignAppService appService, BrandCampaignAttachmentAppService attachmentAppService, IActivityLogAppService activityLogAppService)
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

        public IActionResult Create(BrandCampaigns model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BrandCampaigns model, string submit, IEnumerable<IFormFile> images)
        {
            if (model != null)
            {
                if (model.Title == null)
                {
                    TempData["alert"] = "Judul masih kosong";
                    TempData["success"] = "";
                    return RedirectToAction("Create", model);
                }
                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.Now;
                model.CreatorUsername = this.User.Identity.Name;
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.Now;
                model.DeleterUsername = "";
                
                if (images.Count() > 0)
                {
                    foreach (var image in images)
                    {
                        model.BrandCampaignAttachments.Add(await InsertToAzure(image, model, "Create"));
                    }
                    model.FeaturedImageUrl = model.BrandCampaignAttachments.FirstOrDefault().StorageUrl;
                }

                _appService.Create(model);
            }
            return RedirectToAction("Index");
        }

        private async Task<BrandCampaignAttachments> InsertToAzure(IFormFile file, BrandCampaigns model, string mode)
        {
            BrandCampaignAttachments attachments = new BrandCampaignAttachments();

            //var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

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
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("brandcampaigns");
                

                string namaFile = "";
                string order = "";

                var path = Path.GetExtension(file.FileName);
                if (model.BrandCampaignAttachments.Count == 0)
                {
                    namaFile = "IMG_" + model.Id + "_" + DateTime.Now.ToString("yyyyMMdd") + "_1" + path;
                    order = "1";
                }
                else
                {
                    if (mode == "Create")
                    {
                        order = (int.Parse(model.BrandCampaignAttachments.OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
                    }
                    else
                    {
                        order = (int.Parse(_appService.GetAllAttachments(model.Id).OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
                    }


                    namaFile = "IMG_" + model.Id + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + order + path;
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
                attachments.BrandCampaignId = model.Id;
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
        public IActionResult Edit(BrandCampaigns model, string submit)
        {
            if (model != null)
            {
                if (model.Title == null)
                {
                    TempData["alert"] = "Judul masih kosong";
                    TempData["success"] = "";
                    return RedirectToAction("Create", model.Id);
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
        public IActionResult EditAttachment([DataSourceRequest]DataSourceRequest request, string submit, Guid Id, IEnumerable<IFormFile> files)
        {
            var model = _appService.GetById(Id);

            if (model != null)
            {
                if (files.Count() > 0)
                {
                    foreach (var file in files)
                    {
                        //model.BrandCampaignAttachments.Add(await InsertToAzure(file, model, "Edit"));

                        var newFile = InsertToAzure(file, model, "Edit").Result;

                        _attachmentAppService.Create(newFile);
                    }

                    model = _appService.GetById(Id);
                    model.LastModifierUsername = this.User.Identity.Name;
                    model.LastModificationTime = DateTime.Now;
                    if (model.FeaturedImageUrl == "" || model.FeaturedImageUrl == null)
                    {
                        model.FeaturedImageUrl = model.BrandCampaignAttachments.FirstOrDefault(x => string.IsNullOrEmpty(x.DeleterUsername)).StorageUrl;
                    }
                    _appService.Update(model);
                }
                ViewBag.result = "Berhasil menambahkan attachment";
            }
            return RedirectToAction("EditAttachment", model.Id);
            //return View(model);
        }

        public async Task<IActionResult> Grid_Read([DataSourceRequest]DataSourceRequest request)
        {
            //DataSourceResult result = _appService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderByDescending(x=>x.CreationTime).ToDataSourceResult(request);

            var model = _appService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderByDescending(x => x.CreationTime);
            var activities = await _activityLogAppService.GetContentActivityLogsSummary(null, null, "brandcampaign");
            foreach (var item in model)
            {
                int viewCount = (activities.FirstOrDefault(x => x.ContentId == item.Id.ToString())?.Count ?? 0);
                item.ViewCount = (long)viewCount;
            }
            DataSourceResult result = model.ToDataSourceResult(request);

            return Json(result);
        }

        public IActionResult Grid_Destroy([DataSourceRequest]DataSourceRequest request, BrandCampaigns item)
        {
            if (ModelState.IsValid)
            {
                _appService.SoftDelete(item.Id,this.User.Identity.Name);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        public IActionResult GridAttachment_Read([DataSourceRequest]DataSourceRequest request, Guid id)
        {
            DataSourceResult result = _appService.GetAllAttachments(id).Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderBy(x => x.Title).ToDataSourceResult(request);
            return Json(result);
        }

        public IActionResult GridAttachment_Destroy([DataSourceRequest]DataSourceRequest request, BrandCampaignAttachments item, Guid modelId)
        {
            if (ModelState.IsValid)
            {
                _attachmentAppService.SoftDelete(item.Id, this.User.Identity.Name);

                var url = _appService.GetById(modelId);
                if (_appService.GetById(modelId).FeaturedImageUrl == item.StorageUrl)
                {
                    var model = _appService.GetById(modelId);
                    model.FeaturedImageUrl = "";

                    if(model.BrandCampaignAttachments.Where(x=>string.IsNullOrEmpty(x.DeleterUsername)).Count() > 0)
                    {
                        model.FeaturedImageUrl = model.BrandCampaignAttachments.FirstOrDefault(x => string.IsNullOrEmpty(x.DeleterUsername)).StorageUrl;
                    }
                }
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }
    }
}