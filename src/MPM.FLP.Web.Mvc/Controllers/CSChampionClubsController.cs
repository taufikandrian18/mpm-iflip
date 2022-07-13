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
using MPM.FLP.Web.Models.FLPMPM;

namespace MPM.FLP.Web.Mvc.Controllers
{
    [AbpMvcAuthorize]
    public class CSChampionClubsController : FLPControllerBase
    {
        private readonly UserManager _userManager;
        private readonly CSChampionClubAppService _appService;
        private readonly CSChampionClubParticipantAppService _participantAppService;
        private readonly CSChampionClubAttachmentAppService _attachmentAppService;
        private readonly CSChampionClubRegistrationAppService _regisAppService;
        private readonly InternalUserAppService _userAppService;

        public CSChampionClubsController(UserManager userManager, CSChampionClubAppService appService, CSChampionClubParticipantAppService participantAppService, CSChampionClubAttachmentAppService attachmentAppService, CSChampionClubRegistrationAppService regisAppService, InternalUserAppService userAppService)
        {
            _userManager = userManager;
            _appService = appService;
            _participantAppService = participantAppService;
            _attachmentAppService = attachmentAppService;
            _regisAppService = regisAppService;
            _userAppService = userAppService;
        }

        #region Article
        
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
        public IActionResult Create(CSChampionClubs model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CSChampionClubs model, string submit, IEnumerable<IFormFile> files, IEnumerable<IFormFile> images, IEnumerable<IFormFile> videos)
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
                model.IsRegistered = false;
                model.IsRegistrationCanceled = false;

                foreach (var image in images)
                {
                    model.CSChampionClubAttachments.Add(await InsertToAzure(image, model, "Create"));
                    model.FeaturedImageUrl = model.CSChampionClubAttachments.FirstOrDefault(x => x.Title.Contains("IMG") && string.IsNullOrEmpty(x.DeleterUsername)).StorageUrl;
                }
                foreach (var file in files)
                {
                    model.CSChampionClubAttachments.Add(await InsertToAzure(file, model, "Create"));
                }
                foreach (var video in videos)
                {
                    model.CSChampionClubAttachments.Add(await InsertToAzure(video, model, "Create"));
                }

                _appService.Create(model);
                ViewBag.result = "Data berhasil ditambahkan";

            };

            return RedirectToAction("Index");
        }

        private async Task<CSChampionClubAttachments> InsertToAzure(IFormFile file, CSChampionClubs model, string mode)
        {
            CSChampionClubAttachments attachments = new CSChampionClubAttachments();

            var configuration = new AzureController().GetConnectionToAzure();
            //var configuration = AppConfigurations.Get(AppDomain.CurrentDomain.BaseDirectory);

            string conn = configuration.GetConnectionString(FLPConsts.AzureConnectionString);

            CloudStorageAccount cloudStorage;
            if (CloudStorageAccount.TryParse(conn, out cloudStorage))
            {
                CloudBlobClient cloudBlobClient = cloudStorage.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("cschampionclubs");

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
                if (model.CSChampionClubAttachments.Count == 0)
                {
                    namaFile = fileType + "_" + model.Id + "_" + DateTime.Now.ToString("yyyyMMdd") + "_1" + path;
                    order = "1";
                }
                else
                {
                    if (mode == "Create")
                    {
                        order = (int.Parse(model.CSChampionClubAttachments.OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
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
                attachments.CSChampionClubId = model.Id;
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
        public IActionResult Edit(CSChampionClubs model, string submit)
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

        public IActionResult Participant(CSChampionClubs model)
        {
            return View(model);
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
                    var tmp = _appService.GetById(Id).CSChampionClubAttachments.Where(x => x.Title.Contains("IMG") && string.IsNullOrEmpty(x.DeleterUsername)).Count();
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
        public IActionResult Grid_Read([DataSourceRequest]DataSourceRequest request)
        {
            DataSourceResult result = _appService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderByDescending(x => x.CreationTime).ToDataSourceResult(request);
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

        public IActionResult GridParticipant_Read([DataSourceRequest]DataSourceRequest request, CSChampionClubs model)
        {
            //DataSourceResult result = _participantAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.CSChampionClubId == model.Id).OrderByDescending(x => x.CreationTime).ToDataSourceResult(request);
            var participants = _participantAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) ).OrderByDescending(x => x.CreationTime).ToList();
            List<CSChampionClubParticipantsVM> listParticipant = new List<CSChampionClubParticipantsVM>();
            foreach(var participant in participants)
            {
                CSChampionClubParticipantsVM participantVm = new CSChampionClubParticipantsVM
                {
                    Id = participant.Id,
                    CreationTime = participant.CreationTime,
                    CreatorUsername = participant.CreatorUsername,
                    //CSChampionClubId = participant.CSChampionClubId,
                    DeleterUsername = participant.DeleterUsername,
                    DeletionTime = participant.DeletionTime,
                    IDMPM = participant.IDMPM,
                    LastModificationTime = participant.LastModificationTime,
                    Name = participant.InternalUser.Nama,
                    DealerName = participant.InternalUser.DealerName,
                    LastModifierUsername = participant.LastModifierUsername
                };
                listParticipant.Add(participantVm);
            }
            DataSourceResult result = listParticipant.ToDataSourceResult(request);
            return Json(result);
        }
        public IActionResult GridParticipant_Destroy([DataSourceRequest]DataSourceRequest request, CSChampionClubParticipantsVM item)
        {
            if (ModelState.IsValid)
            {
                _participantAppService.SoftDelete(item.Id, this.User.Identity.Name);
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
                        var attachments = model.CSChampionClubAttachments.Where(y => string.IsNullOrEmpty(y.DeleterUsername));

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
        #endregion

        #region Participants

        public IActionResult Participants()
        {
            TempData["alert"] = "";
            TempData["success"] = "";
            return View();
        }

        public IActionResult SearchParticipants(int year)
        {
            List<CSChampionClubParticipantsVM> result = new List<CSChampionClubParticipantsVM>();
            
            var tmpResults = _participantAppService.GetAll().Where(x => x.Year == year).ToList();
            
            foreach(var tmpResult in tmpResults)
            {
                CSChampionClubParticipantsVM tmp = new CSChampionClubParticipantsVM();
                
                tmp.IDMPM = tmpResult.IDMPM;
                tmp.Name = _userAppService.GetAll().SingleOrDefault(x => x.IDMPM == tmp.IDMPM).Nama;
                tmp.DealerName = _userAppService.GetAll().SingleOrDefault(x => x.IDMPM == tmp.IDMPM).DealerName;

                result.Add(tmp);
            }

            if(result.Count > 0)
                return Json( new { success = true, item = result});
            else
                return Json(new { success = false});
        }

        #endregion

        #region Registration

        public IActionResult Registration()
        {
            var model = _regisAppService.GetAll().SingleOrDefault(x => x.Year == DateTime.UtcNow.Year);

            if(model == null)
            {
                model = new CSChampionClubRegistrations();
                model.Year = DateTime.UtcNow.Year;
                model.StartDate = DateTime.UtcNow.AddHours(7);
                model.EndDate = DateTime.UtcNow.AddHours(7);
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateRegistration(CSChampionClubRegistrations model, string submit)
        {
            var modelOld = _regisAppService.GetAll().SingleOrDefault(x => x.Year == model.Year);
            if (modelOld == null)
            {
                model.CreationTime = DateTime.UtcNow.AddHours(7);
                model.CreatorUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.UtcNow.AddHours(7);
                model.LastModifierUsername = this.User.Identity.Name;

                _regisAppService.Create(model);
            }
            else
            {
                modelOld.StartDate = model.StartDate;
                model.EndDate = model.EndDate;
                model.LastModificationTime = DateTime.UtcNow.AddHours(7);
                model.LastModifierUsername = this.User.Identity.Name;

                _regisAppService.Update(modelOld);
            }
            return Json(new { success = true });
        }

        #endregion
    }
}