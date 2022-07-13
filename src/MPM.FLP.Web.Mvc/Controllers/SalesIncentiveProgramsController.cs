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
    public class SalesIncentiveProgramsController : FLPControllerBase
    {
        private readonly UserManager _userManager;
        private readonly SalesIncentiveProgramAppService _appService;
        private readonly SalesIncentiveProgramAttachmentAppService _attachmentAppService;
        private readonly SalesIncentiveProgramJabatanAppService _incentiveJabatanAppService;
        private readonly SalesIncentiveProgramKotaAppService _incentiveKotaAppService;
        private readonly KotaAppService _kotaAppService;
        private readonly JabatanAppService _jabatanAppService;
        private readonly InternalUserAppService _internalUserAppService;

        public SalesIncentiveProgramsController(SalesIncentiveProgramAppService appService, SalesIncentiveProgramAttachmentAppService attachmentAppService, UserManager userManager, SalesIncentiveProgramJabatanAppService incentiveJabatanAppService, SalesIncentiveProgramKotaAppService incentiveKotaAppService, KotaAppService kotaAppService, JabatanAppService jabatanAppService, InternalUserAppService internalUserAppService)
        {
            _appService = appService;
            _attachmentAppService = attachmentAppService;
            _incentiveJabatanAppService = incentiveJabatanAppService;
            _userManager = userManager;
            _incentiveKotaAppService = incentiveKotaAppService;
            _kotaAppService = kotaAppService;
            _jabatanAppService = jabatanAppService;
            _internalUserAppService = internalUserAppService;
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
        public IActionResult Create(SalesIncentivePrograms model)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == this.User.Identity.GetUserId());
            var roles = _userManager.GetRolesAsync(user).Result.ToList();

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Create(SalesIncentivePrograms model, string submit, IEnumerable<IFormFile> files, IEnumerable<IFormFile> images, IEnumerable<IFormFile> videos)
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
                        model.SalesIncentiveProgramAttachments.Add(await InsertToAzure(file, model, "Create"));
                    }

                    model.FeaturedImageUrl = model.SalesIncentiveProgramAttachments.First().StorageUrl;
                }
                if (files.Count() > 0)
                {
                    foreach (var file in files)
                    {
                        model.SalesIncentiveProgramAttachments.Add(await InsertToAzure(file, model, "Create"));
                    }
                }
                if (videos.Count() > 0)
                {
                    foreach (var file in videos)
                    {
                        model.SalesIncentiveProgramAttachments.Add(await InsertToAzure(file, model, "Create"));
                    }
                }

                _appService.Create(model);
            }
            //return RedirectToAction("Index");
            return Json(new { success = true, message = model.Id });
        }

        public IActionResult IncentiveUser(Guid id)
        {
            List<Kotas> listKota = _kotaAppService.GetAll().ToList();
            List<Jabatans> listJabatan = _jabatanAppService.GetAll().Where(x=>x.Channel == "H1").ToList();
            List<string> incentiveKota = _incentiveKotaAppService.GetAll().Where(x => x.SalesIncentiveProgramId == id).Select(x=>x.NamaKota).ToList();
            List<string> incentiveJabatan = _incentiveJabatanAppService.GetAll().Where(x => x.SalesIncentiveProgramId == id).Select(x => x.NamaJabatan).ToList();
            List<IncentiveKotasVM> listIncetiveKota = new List<IncentiveKotasVM>();
            List<IncentiveJabatanVM> listIncetiveJabatan = new List<IncentiveJabatanVM>();

            foreach (var kota in listKota)
            {
                bool isCheck = incentiveKota.Contains(kota.NamaKota);
                var item = new IncentiveKotasVM
                {
                    Id = kota.Id,
                    NamaKota = kota.NamaKota,
                    isChecked = isCheck
                };
                listIncetiveKota.Add(item);
            }

            foreach (var jabatan in listJabatan)
            {
                bool isCheck = incentiveJabatan.Contains(jabatan.Nama);
                var item = new IncentiveJabatanVM
                {
                    Id = jabatan.Id,
                    NamaJabatan = jabatan.Nama,
                    isChecked = isCheck
                };
                listIncetiveJabatan.Add(item);
            }

            IncentiveUserVM incentiveUser = new IncentiveUserVM
            {
                Id = id,
                Title = _appService.GetById(id).Title,
                ListKota = listIncetiveKota,
                ListJabatan = listIncetiveJabatan
            };

            return View(incentiveUser);
        }

        public IActionResult InsertIncentiveUser(IncentiveUserVM model)
        {
            List<IncentiveKotasVM> selectedKota = model.ListKota.Where(x => x.isChecked == true).ToList();
            List<IncentiveJabatanVM> selectedJabatan = model.ListJabatan.Where(x => x.isChecked == true).ToList();

            List<SalesIncentiveProgramKotas> existedKota = _incentiveKotaAppService.GetAll().Where(x => x.SalesIncentiveProgramId == model.Id).Select(x => x).ToList();
            List<SalesIncentiveProgramJabatans> existedJabatan = _incentiveJabatanAppService.GetAll().Where(x => x.SalesIncentiveProgramId == model.Id).Select(x => x).ToList();

            if(existedKota.Count == 0)
            {
                foreach(var kota in selectedKota)
                {
                    SalesIncentiveProgramKotas newKota = new SalesIncentiveProgramKotas
                    {
                        Id = Guid.NewGuid(),
                        NamaKota = kota.NamaKota,
                        SalesIncentiveProgramId = model.Id
                    };
                    _incentiveKotaAppService.Create(newKota);
                }
            }
            else
            {
                var slices = selectedKota.Where(s => !existedKota.Where(es => es.NamaKota == s.NamaKota).Any());
                var notInSlices = selectedKota.Where(s => !existedKota.Where(es => es.NamaKota == s.NamaKota).Any());
                foreach (var kota in slices)
                {
                    SalesIncentiveProgramKotas newKota = new SalesIncentiveProgramKotas
                    {
                        Id = Guid.NewGuid(),
                        NamaKota = kota.NamaKota,
                        SalesIncentiveProgramId = model.Id
                    };
                    _incentiveKotaAppService.Create(newKota);
                }
                foreach (var kota in notInSlices)
                {
                    var id = existedKota.Where(x => x.NamaKota == kota.NamaKota).Select(x => x.Id).SingleOrDefault();
                    _incentiveKotaAppService.Delete(id);
                }
            }
            if (existedJabatan.Count == 0)
            {
                foreach (var jabatan in selectedJabatan)
                {
                    SalesIncentiveProgramJabatans newJabatan = new SalesIncentiveProgramJabatans
                    {
                        Id = Guid.NewGuid(),
                        NamaJabatan = jabatan.NamaJabatan,
                        SalesIncentiveProgramId = model.Id
                    };
                    _incentiveJabatanAppService.Create(newJabatan);
                }
            }
            else
            {
                var slices = selectedJabatan.Where(s => !existedJabatan.Where(es => es.NamaJabatan == s.NamaJabatan).Any());
                var notInSlices = existedJabatan.Where(s => !selectedJabatan.Where(es => es.NamaJabatan == s.NamaJabatan).Any());
                foreach (var jabatan in slices)
                {
                    SalesIncentiveProgramJabatans newJabatan = new SalesIncentiveProgramJabatans
                    {
                        Id = Guid.NewGuid(),
                        NamaJabatan = jabatan.NamaJabatan,
                        SalesIncentiveProgramId = model.Id
                    };
                    _incentiveJabatanAppService.Create(newJabatan);
                }
                foreach (var jabatan in notInSlices)
                {
                    _incentiveJabatanAppService.Delete(jabatan.Id);
                }
            }

            return Json(new { success = true });
        }

        private async Task<SalesIncentiveProgramAttachments> InsertToAzure(IFormFile file, SalesIncentivePrograms model, string mode)
        {
            SalesIncentiveProgramAttachments attachments = new SalesIncentiveProgramAttachments();

            var configuration = new AzureController().GetConnectionToAzure();

            string conn = configuration.GetConnectionString(FLPConsts.AzureConnectionString);

            CloudStorageAccount cloudStorage;
            if (CloudStorageAccount.TryParse(conn, out cloudStorage))
            {
                CloudBlobClient cloudBlobClient = cloudStorage.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("salesincentiveprograms");

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
                if (model.SalesIncentiveProgramAttachments.Count == 0)
                {
                    namaFile = fileType + "_"+ file.FileName + "_" + model.Id + "_" + DateTime.UtcNow.AddHours(7).ToString("yyyyMMdd") + "_1" + path;
                    order = "1";
                }
                else
                {
                    if (mode == "Create")
                    {
                        order = (int.Parse(model.SalesIncentiveProgramAttachments.OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
                    }
                    else
                    {
                        order = (int.Parse(_appService.GetById(model.Id).SalesIncentiveProgramAttachments.OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
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
                attachments.SalesIncentiveProgramId = model.Id;
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

        public IActionResult Grid_ReadAttachment([DataSourceRequest]DataSourceRequest request, InboxMessages model)
        {
            var tmp = _appService.GetById(model.Id);
            return Json(tmp.SalesIncentiveProgramAttachments.ToDataSourceResult(request));
        }

        [HttpPost]
        public IActionResult Edit(SalesIncentivePrograms model, string submit)
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
            return Json(new { success = true });
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
                    var tmp = _appService.GetById(Id).SalesIncentiveProgramAttachments.Where(x => x.Title.Contains("IMG") && string.IsNullOrEmpty(x.DeleterUsername)).Count();
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
                        model.FeaturedImageUrl = _appService.GetById(Id).SalesIncentiveProgramAttachments.FirstOrDefault(x => x.Title.Contains("IMG") && string.IsNullOrEmpty(x.DeleterUsername)).StorageUrl;
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

            DataSourceResult result = _appService.GetAll().Where(x =>  string.IsNullOrEmpty(x.DeleterUsername)).OrderByDescending(x=>x.CreationTime).ToDataSourceResult(request);

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

        public IActionResult GridAttachmentImage_Read([DataSourceRequest]DataSourceRequest request, Guides model)
        {
            DataSourceResult result = _appService.GetById(model.Id).SalesIncentiveProgramAttachments.Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.Title.Contains("IMG")).OrderBy(x => x.Title).ToDataSourceResult(request);
            return Json(result);
        }
        public IActionResult GridAttachmentDocument_Read([DataSourceRequest]DataSourceRequest request, Guides model)
        {
            DataSourceResult result = _appService.GetById(model.Id).SalesIncentiveProgramAttachments.Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.Title.Contains("DOC")).OrderBy(x => x.Title).ToDataSourceResult(request);
            return Json(result);
        }
        public IActionResult GridAttachmentVideo_Read([DataSourceRequest]DataSourceRequest request, Guides model)
        {
            DataSourceResult result = _appService.GetById(model.Id).SalesIncentiveProgramAttachments.Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.Title.Contains("VID")).OrderBy(x => x.Title).ToDataSourceResult(request);

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
                        var attachments = model.SalesIncentiveProgramAttachments.Where(y => string.IsNullOrEmpty(y.DeleterUsername));

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