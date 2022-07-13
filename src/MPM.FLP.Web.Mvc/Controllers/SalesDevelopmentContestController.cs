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
using Microsoft.AspNetCore.Mvc.Rendering;
using MPM.FLP.Web.Models.FLPMPM;

namespace MPM.FLP.Web.Mvc.Controllers
{
    [AbpMvcAuthorize]
    public class SalesDevelopmentContestController : FLPControllerBase
    {
        private readonly UserManager _userManager;
        private readonly InboxMessageAppService _appService;
        private readonly InboxAttachmentAppService _attachmentAppService;
        private readonly InboxRecipientAppService _recipientAppService;
        private readonly DealerAppService _dealerAppService;
        private readonly InternalUserAppService _internalUserAppService;

        public SalesDevelopmentContestController(InboxMessageAppService appService, InboxAttachmentAppService attachmentAppService, UserManager userManager, InboxRecipientAppService recipientAppService, DealerAppService dealerAppService, InternalUserAppService internalUserAppService)
        {
            _appService = appService;
            _attachmentAppService = attachmentAppService;
            _recipientAppService = recipientAppService;
            _userManager = userManager;
            _dealerAppService = dealerAppService;
            _internalUserAppService = internalUserAppService;
        }

        public IActionResult Index()
        {
            TempData["alert"] = "";
            TempData["success"] = "";
            
            return View();
        }

        public IActionResult Cascading_Get_Channel([DataSourceRequest]DataSourceRequest request)
        {
            var channel = _dealerAppService.GetChannel();
            DataSourceResult result = channel.Select(c => new { channel = c }).ToList().ToDataSourceResult(request);


            return Json(result);
            //return Json(karesidenan.Select(c => new { karesidenan = c }).ToList());
        }

        public IActionResult Cascading_Get_Karesidenan([DataSourceRequest]DataSourceRequest request, string channel)
        {
            DataSourceResult result = new DataSourceResult();
            if (string.IsNullOrEmpty(channel))
            {
                var karesidenan = _dealerAppService.GetKaresidenanH1();
                result = karesidenan.Select(c => new { karesidenan = c }).ToList().ToDataSourceResult(request);
            }
            else
            {
                List<string> karesidenan = _dealerAppService.GetKaresidenanHC3(channel);
                
                result = karesidenan.Select(c => new { channel = channel,karesidenan = c }).ToList().ToDataSourceResult(request);
            }
            

            return Json(result);
            //return Json(karesidenan.Select(c => new { karesidenan = c }).ToList());
        }
        public IActionResult Cascading_Get_Kota([DataSourceRequest]DataSourceRequest request, string channel,string karesidenan)
        {
            DataSourceResult result = new DataSourceResult();
            if (string.IsNullOrEmpty(karesidenan) && string.IsNullOrEmpty(channel))
            {
                var kota = _dealerAppService.GetKotaH2().Select(x => new { kota = x }).ToList();
                result = kota.ToDataSourceResult(request);
            }
            else if(string.IsNullOrEmpty(channel))
            {
                var kota = _dealerAppService.GetKotaH1(karesidenan).Select(x => new { karesidenan = karesidenan, kota = x }).ToList();
                result = kota.ToDataSourceResult(request);
            }
            else
            {
                var kota = _dealerAppService.GetKotaHC3(channel,karesidenan).Select(x => new { karesidenan = karesidenan, kota = x }).ToList();
                result = kota.ToDataSourceResult(request);
            }
            
            return Json(result);
            //var kota = _dealerAppService.GetKotaH1(karesidenan);

            //return Json(karesidenan.Select(c => new { kota = c }).ToList());
        }
        public IActionResult Cascading_Get_Dealer([DataSourceRequest]DataSourceRequest request, string channel, string karesidenan, string kota)
        {
            var dealer = _dealerAppService.GetAll().Where(x=>x.Channel == channel && x.Kota == kota).Select(x => new { kota = x.Kota ,dealer = x.Nama }).ToList();
            DataSourceResult result = dealer.ToDataSourceResult(request);

            return Json(result);
            //var kota = _dealerAppService.GetKotaH1(karesidenan);

            //return Json(karesidenan.Select(c => new { kota = c }).ToList());
        }

        public IActionResult Get_User([DataSourceRequest]DataSourceRequest request, string channel, string karesidenan, string kota, string dealer)
        {
            List<InternalUsers> internalUser = new List<InternalUsers>();

            var user = _userManager.Users.FirstOrDefault(x => x.Id == this.User.Identity.GetUserId());
            var roles = _userManager.GetRolesAsync(user).Result.ToList();

            string resource = "";
            if (roles.FirstOrDefault().Contains("H1"))
            {
                resource = "H1";
                if (string.IsNullOrEmpty(kota) && string.IsNullOrEmpty(dealer))
                {
                    internalUser = _internalUserAppService.GetInternalUserByKaresidenanH1(karesidenan).ToList();
                }
                else if (string.IsNullOrEmpty(dealer))
                {
                    internalUser = _internalUserAppService.GetInternalUserByKota(resource, kota).ToList();
                }
                else
                {
                    internalUser = _internalUserAppService.GetAll().Where(x => x.Channel == resource && x.DealerKota == kota && x.DealerName == dealer).ToList();
                }
            }
            else if (roles.FirstOrDefault().Contains("H2"))
            {
                resource = "H2";
                
                if (string.IsNullOrEmpty(dealer))
                {
                    internalUser = _internalUserAppService.GetInternalUserByKota(resource, kota).ToList();
                }
                else
                {
                    internalUser = _internalUserAppService.GetAll().Where(x => x.Channel == resource && x.DealerKota == kota && x.DealerName == dealer).ToList();
                }
            }
            else if (roles.FirstOrDefault().Contains("HC3"))
            {
                resource = "HC3";
                if (string.IsNullOrEmpty(karesidenan) && string.IsNullOrEmpty(kota) && string.IsNullOrEmpty(dealer))
                {
                    internalUser = _internalUserAppService.GetInternalUserByChannel(channel).ToList();
                }
                else if (string.IsNullOrEmpty(kota) && string.IsNullOrEmpty(dealer))
                {
                    internalUser = _internalUserAppService.GetInternalUserByKaresidenanHC3(channel,karesidenan).ToList();
                }
                else if (string.IsNullOrEmpty(dealer))
                {
                    internalUser = _internalUserAppService.GetInternalUserByKota(channel, kota).ToList();
                }
                else
                {
                    internalUser = _internalUserAppService.GetAll().Where(x => x.Channel == channel && x.DealerKota == kota && x.DealerName == dealer).ToList();
                }
            }
            
            //return Json(internalUser.ToDataSourceResult(request));
            return Json(internalUser);
        }

        //public IActionResult Create()
        //{
        //    Articles model = new Articles();
        //    return View(model);
        //}
        public IActionResult Create(InboxMessages model)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == this.User.Identity.GetUserId());
            var roles = _userManager.GetRolesAsync(user).Result.ToList();

            if (roles.FirstOrDefault().Contains("H1"))
            {
                return View(model);
            }
            else if (roles.FirstOrDefault().Contains("H2"))
            {
                //return RedirectToAction("CreateH2", model);
                return View("CreateH2",model);
            }
            else if (roles.FirstOrDefault().Contains("HC3"))
            {
                //return RedirectToAction("CreateHC3", model);
                return View("CreateHC3", model);
            }

            return View(model);
        }

        public IActionResult CreateH2(InboxMessages model)
        {
            return View(model);
        }

        public IActionResult CreateHC3(InboxMessages model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(InboxMessages model, string submit, IEnumerable<IFormFile> files, IEnumerable<IFormFile> images, IEnumerable<IFormFile> videos)
        {
            if (model != null)
            {
                if (model.Title == null)
                {
                    TempData["alert"] = "Judul masih kosong";
                    TempData["success"] = "";
                    //return RedirectToAction("Create", model);
                    return Json(new { success = false, itemModel = model });
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
                model.CreationTime = DateTime.UtcNow.AddHours(7);
                model.CreatorUsername = this.User.Identity.Name;
                model.LastModifierUsername = this.User.Identity.Name;
                model.LastModificationTime = DateTime.UtcNow.AddHours(7);
                model.DeleterUsername = "";

                if (images.Count() > 0)
                {
                    foreach (var file in images)
                    {
                        model.InboxAttachments.Add(await InsertToAzure(file, model, "Create"));
                    }

                    model.FeaturedImageUrl = model.InboxAttachments.First().StorageUrl;
                }
                if (files.Count() > 0)
                {
                    foreach (var file in files)
                    {
                        model.InboxAttachments.Add(await InsertToAzure(file, model, "Create"));
                    }
                }
                if (videos.Count() > 0)
                {
                    foreach (var file in videos)
                    {
                        model.InboxAttachments.Add(await InsertToAzure(file, model, "Create"));
                    }
                }

                _appService.Create(model);
            }
            //return RedirectToAction("Index");
            return Json(new { success = true, message = model.Id });
        }

        [HttpPost]
        public ActionResult InsertRecipient(string id, List<InternalUsersVM> selectedUser)
        {
            var model = _appService.GetById(Guid.Parse(id));

            foreach(var user in selectedUser)
            {
                model.InboxRecipients.Add(new InboxRecipients { 
                    Id = Guid.NewGuid(),
                    CreationTime = DateTime.UtcNow.AddHours(7),
                    CreatorUsername = this.User.Identity.Name,
                    LastModifierUsername = this.User.Identity.Name,
                    DeleterUsername = "",
                    IDMPM = user.Idmpm,
                    InboxMessageId = model.Id,
                    IsRead = false
                });
            }

            _appService.Update(model);


            return Json(new { success = true, message = "Data berhasil dimasukkan" });
        }



        private async Task<InboxAttachments> InsertToAzure(IFormFile file, InboxMessages model, string mode)
        {
            InboxAttachments attachments = new InboxAttachments();

            var configuration = new AzureController().GetConnectionToAzure();

            string conn = configuration.GetConnectionString(FLPConsts.AzureConnectionString);

            CloudStorageAccount cloudStorage;
            if (CloudStorageAccount.TryParse(conn, out cloudStorage))
            {
                CloudBlobClient cloudBlobClient = cloudStorage.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("inbox");

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
                if (model.InboxAttachments.Count == 0)
                {
                    namaFile = fileType + "_"+ file.FileName + "_" + model.Id + "_" + DateTime.UtcNow.AddHours(7).ToString("yyyyMMdd") + "_1" + path;
                    order = "1";
                }
                else
                {
                    if (mode == "Create")
                    {
                        order = (int.Parse(model.InboxAttachments.OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
                    }
                    else
                    {
                        order = (int.Parse(_appService.GetAllAttachments(model.Id).OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
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
                attachments.InboxMessageId = model.Id;
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

        public IActionResult GetUsers([DataSourceRequest]DataSourceRequest request, InboxMessages model)
        {
            var tmp = _appService.GetById(model.Id);
            List<InternalUsersVM> internalUsers = new List<InternalUsersVM>();
            foreach(var recipient in tmp.InboxRecipients)
            {
                var user = _internalUserAppService.GetAll().SingleOrDefault(x => x.IDMPM == recipient.IDMPM);
                internalUsers.Add(new InternalUsersVM()
                {
                    Idmpm = user.IDMPM,
                    Nama = user.Nama
                });
            }

            return Json(internalUsers.ToDataSourceResult(request));
        }
        public IActionResult Grid_ReadAttachment([DataSourceRequest]DataSourceRequest request, InboxMessages model)
        {
            var tmp = _appService.GetById(model.Id);
            return Json(tmp.InboxAttachments.ToDataSourceResult(request));
        }

        [HttpPost]
        public IActionResult Edit(InboxMessages model, string submit)
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
                model.LastModificationTime = DateTime.UtcNow.AddHours(7);

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
                    //foreach (var file in files)
                    //{
                    //    //model.ArticleAttachments.Add(await InsertToAzure(file, model, "Edit"));
                    //    var newFile = InsertToAzure(file, model, "Edit").Result;

                    //    _attachmentAppService.Create(newFile);
                    //}
                    model = _appService.GetById(Id);
                    //Check if featuredimgurl empty
                    if (string.IsNullOrEmpty(model.FeaturedImageUrl))
                    {
                        model.FeaturedImageUrl = _appService.GetAllAttachments(Id).FirstOrDefault(x => string.IsNullOrEmpty(x.DeleterUsername)).StorageUrl;
                    }
                    model.LastModifierUsername = this.User.Identity.Name;
                    model.LastModificationTime = DateTime.UtcNow.AddHours(7);
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

        public IActionResult Grid_Read([DataSourceRequest]DataSourceRequest request)
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

            DataSourceResult result = _appService.GetAll().Where(x =>  string.IsNullOrEmpty(x.DeleterUsername) && x.CreatorUsername.ToLower() == this.User.Identity.Name.ToLower()).OrderByDescending(x=>x.CreationTime).ToDataSourceResult(request);

            return Json(result);
        }

        public IActionResult Grid_Destroy([DataSourceRequest]DataSourceRequest request, InboxMessages item)
        {
            if (ModelState.IsValid)
            {
                _appService.SoftDelete(item.Id,this.User.Identity.Name);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        public IActionResult GridAttachment_Read([DataSourceRequest]DataSourceRequest request, Guid modelId)
        {
            DataSourceResult result = _appService.GetAllAttachments(modelId).Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderBy(x => x.Title).ToDataSourceResult(request);
            return Json(result);
        }

        public IActionResult GridAttachment_Destroy([DataSourceRequest]DataSourceRequest request, InboxAttachments item, Guid modelId)
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
                        var attachments = model.InboxAttachments.Where(y => string.IsNullOrEmpty(y.DeleterUsername));

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