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
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Web;
using MPM.FLP.Services.Dto;
using MPM.FLP.Web.Models.FLPMPM;

namespace MPM.FLP.Web.Mvc.Controllers
{
    public class ClaimProgramsController : FLPControllerBase
    {
        private readonly ClaimProgramAppService _appService;
        private readonly ClaimProgramAttachmentAppService _attachmentAppService;
        private readonly ClaimProgramClaimerAppService _claimerAppService;

        public ClaimProgramsController(ClaimProgramAppService appService, ClaimProgramClaimerAppService claimerAppService, ClaimProgramAttachmentAppService attachmentAppService)
        {
            _appService = appService;
            _claimerAppService = claimerAppService;
            _attachmentAppService = attachmentAppService;
        }

        #region Main

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            ClaimProgramCreateVM model = new ClaimProgramCreateVM();
            model.IsH3 = false;
            model.IsH3Ahass = false;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ClaimProgramCreateVM _model, string submit, IEnumerable<IFormFile> files, IEnumerable<IFormFile> images, IEnumerable<IFormFile> videos)
        {
            if(_model != null)
            {
                ClaimPrograms model = new ClaimPrograms {
                    Id = Guid.NewGuid(),
                    CreationTime = DateTime.Now,
                    CreatorUsername = this.User.Identity.Name,
                    LastModifierUsername = this.User.Identity.Name,
                    LastModificationTime = DateTime.Now,
                    DeleterUsername = "",
                    FeaturedImageUrl = "",
                    Title = _model.Title,
                    Contents = _model.Contents,
                    IsDoku = _model.IsDoku,
                    IsH3 = _model.IsH3,
                    IsH3Ahass = _model.IsH3Ahass,
                    IsPublished = _model.IsPublished,
                    NonDokuReward = _model.NonDokuReward,
                    DokuReward = _model.DokuReward,
                    StartDate = _model.StartDate,
                    EndDate = _model.EndDate,
                };
                

                foreach (var image in images)
                {
                    model.ClaimProgramAttachments.Add(await InsertToAzure(image, model, "Create"));
                    model.FeaturedImageUrl = model.ClaimProgramAttachments.Where(x => x.Title.Contains("IMG")).FirstOrDefault().StorageUrl;
                }
                foreach (var file in files)
                {
                    model.ClaimProgramAttachments.Add(await InsertToAzure(file, model, "Create"));
                }
                foreach (var video in videos)
                {
                    model.ClaimProgramAttachments.Add(await InsertToAzure(video, model, "Create"));
                }

                _appService.Create(model);
            }
            return Json(new { success = true});
        }

        public IActionResult Edit(Guid id)
        {
            var item = _appService.GetById(id);
            item.Contents = HttpUtility.HtmlDecode(item.Contents);

            ClaimProgramCreateVM model = new ClaimProgramCreateVM
            {
                Id = item.Id,
                CreationTime = item.CreationTime,
                CreatorUsername = item.CreatorUsername,
                LastModifierUsername = item.LastModifierUsername,
                LastModificationTime = item.LastModificationTime,
                DeleterUsername = item.DeleterUsername,
                FeaturedImageUrl = item.FeaturedImageUrl,
                Title = item.Title,
                Contents = item.Contents,
                IsDoku = item.IsDoku,
                IsH3 = item.IsH3.Value,
                IsH3Ahass = item.IsH3Ahass.Value,
                IsPublished = item.IsPublished,
                NonDokuReward = item.NonDokuReward,
                DokuReward = item.DokuReward,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(ClaimProgramCreateVM item, string submit)
        {
            ClaimPrograms model = new ClaimPrograms
            {
                Id = item.Id,
                CreationTime = item.CreationTime,
                CreatorUsername = item.CreatorUsername,
                LastModifierUsername = item.LastModifierUsername,
                LastModificationTime = item.LastModificationTime,
                DeleterUsername = item.DeleterUsername,
                FeaturedImageUrl = item.FeaturedImageUrl,
                Title = item.Title,
                Contents = item.Contents,
                IsDoku = item.IsDoku,
                IsH3 = item.IsH3,
                IsH3Ahass = item.IsH3Ahass,
                IsPublished = item.IsPublished,
                NonDokuReward = item.NonDokuReward,
                DokuReward = item.DokuReward,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
            };

            if (model != null)
            {
                if(model.FeaturedImageUrl == null)
                {
                    model.FeaturedImageUrl = "";
                }
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.Now;

                _appService.Update(model);
            }
            return Json(new { success = true });
        }

        public IActionResult Grid_Read([DataSourceRequest]DataSourceRequest request)
        {

            DataSourceResult result = _appService.GetAll().Where(x=>string.IsNullOrEmpty(x.DeleterUsername)).OrderByDescending(x=>x.CreationTime).ToDataSourceResult(request);

            return Json(result);
        }

        public IActionResult Grid_Destroy([DataSourceRequest]DataSourceRequest request, ClaimPrograms item)
        {
            if (ModelState.IsValid)
            {
                _appService.SoftDelete(item.Id, this.User.Identity.Name);
            }
            
            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        private async Task<ClaimProgramAttachments> InsertToAzure(IFormFile file, ClaimPrograms model, string mode)
        {
            ClaimProgramAttachments attachments = new ClaimProgramAttachments();

            var configuration = new AzureController().GetConnectionToAzure();

            string conn = configuration.GetConnectionString(FLPConsts.AzureConnectionString);

            CloudStorageAccount cloudStorage;
            if (CloudStorageAccount.TryParse(conn, out cloudStorage))
            {
                CloudBlobClient cloudBlobClient = cloudStorage.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("claimprograms");

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
                if (model.ClaimProgramAttachments.Count == 0)
                {
                    namaFile = fileType + "_" + file.FileName + "_" + model.Id + "_" + DateTime.UtcNow.AddHours(7).ToString("yyyyMMdd") + "_1" + path;
                    order = "1";
                }
                else
                {
                    if (mode == "Create")
                    {
                        order = (int.Parse(model.ClaimProgramAttachments.OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
                    }
                    else
                    {
                        order = (int.Parse(_appService.GetById(model.Id).ClaimProgramAttachments.OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
                    }

                    namaFile = fileType + "_" + model.Id + "_" + DateTime.UtcNow.AddHours(7).ToString("yyyyMMdd") + "_" + order + path;
                }

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(namaFile);

                //await cloudBlockBlob.UploadFromFileAsync(file.FileName);
                using (Stream stream = file.OpenReadStream())
                {
                    stream.Position = 0;
                    await cloudBlockBlob.UploadFromStreamAsync(stream);
                }

                attachments.Id = Guid.NewGuid();
                attachments.CreationTime = DateTime.UtcNow.AddHours(7);
                attachments.CreatorUsername = this.User.Identity.Name;
                attachments.LastModifierUsername = this.User.Identity.Name;
                attachments.DeleterUsername = null;
                attachments.ClaimProgramId = model.Id;
                attachments.Order = order;
                attachments.Title = namaFile;
                attachments.StorageUrl = cloudBlockBlob.Uri.AbsoluteUri;
                attachments.FileName = file.FileName;
            }

            return attachments;
        }
        
        #endregion
        #region Claimer

        public IActionResult ClaimedPeople(Guid id)
        {
            var model = _appService.GetById(id);
            return View(model);
        }
        public IActionResult Grid_Claimer_Read([DataSourceRequest]DataSourceRequest request, ClaimPrograms model)
        {

            DataSourceResult result = _claimerAppService.GetClaimers(model.Id).ToDataSourceResult(request);

            return Json(result);
        }

        public IActionResult Grid_Claimer_Destroy([DataSourceRequest]DataSourceRequest request, ClaimerDto item)
        {
            if (ModelState.IsValid)
            {
                //_claimerAppService.SoftDelete(item.Id);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }
        public IActionResult ApproveClaim(Guid id)
        {
            _claimerAppService.Approve(new ApprovalClaimDto { ClaimProgramClaimerId = id, IsApproved = true });

            return Json(new { success = true });
        }

        public IActionResult RejectClaim(Guid id)
        {
            _claimerAppService.Approve(new ApprovalClaimDto { ClaimProgramClaimerId = id, IsApproved = false });

            return Json(new { success = true });
        }
        #endregion

        #region Attachment
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
                    var tmp = _appService.GetById(Id).ClaimProgramAttachments.Where(x => x.Title.Contains("IMG") && string.IsNullOrEmpty(x.DeleterUsername)).Count();
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
                        var attachments = model.ClaimProgramAttachments.Where(y => string.IsNullOrEmpty(y.DeleterUsername));

                        if (attachments.Count() > 0 && attachments.Select(x => x.Title).Any(x => x.Contains("IMG")))
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
        #endregion

        #region Excel
        
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
        #endregion
    }
}