using Microsoft.AspNetCore.Mvc;
using MPM.FLP.Services;
using System.Collections.Generic;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System.IO;
using MPM.FLP.Authorization.Users;
using MPM.FLP.Services.Backoffice;
using System.Diagnostics;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Microsoft.AspNetCore.Hosting;
using MPM.FLP.Services.Dto;

namespace MPM.FLP.Services.Backoffice
{
    public class SalesIncentiveProgramsController : FLPAppServiceBase, ISalesIncentiveProgramsController
    {
        private readonly UserManager _userManager;
        private readonly SalesIncentiveProgramAppService _appService;
        private readonly SalesIncentiveProgramAttachmentAppService _attachmentAppService;
        private readonly SalesIncentiveProgramJabatanAppService _incentiveJabatanAppService;
        private readonly SalesIncentiveProgramKotaAppService _incentiveKotaAppService;
        private readonly SalesIncentiveProgramTargetAppService _incentiveTargetAppService;
        private readonly KotaAppService _kotaAppService;
        private readonly JabatanAppService _jabatanAppService;
        private readonly InternalUserAppService _internalUserAppService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public SalesIncentiveProgramsController(
            SalesIncentiveProgramAppService appService, 
            SalesIncentiveProgramAttachmentAppService attachmentAppService, 
            UserManager userManager, 
            SalesIncentiveProgramJabatanAppService incentiveJabatanAppService, 
            SalesIncentiveProgramKotaAppService incentiveKotaAppService,
            SalesIncentiveProgramTargetAppService incentiveTargetAppService,
            KotaAppService kotaAppService, 
            JabatanAppService jabatanAppService, 
            InternalUserAppService internalUserAppService, 
            IHostingEnvironment hostingEnvironment)
        {
            _appService = appService;
            _attachmentAppService = attachmentAppService;
            _incentiveJabatanAppService = incentiveJabatanAppService;
            _userManager = userManager;
            _incentiveKotaAppService = incentiveKotaAppService;
            _kotaAppService = kotaAppService;
            _incentiveTargetAppService = incentiveTargetAppService;
            _jabatanAppService = jabatanAppService;
            _internalUserAppService = internalUserAppService;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet("/api/services/app/backoffice/SalesIncentivePrograms/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _appService.GetAll();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Title.Contains(request.Query) || x.Contents.Contains(request.Query) || x.CreatorUsername.Contains(request.Query));
            }

            var count = query.Count();

            query = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit);

            var data = new List<SalesIncentivePrograms>();

            try
            {
                data = query.ToList();
            }
            catch (System.Data.SqlTypes.SqlNullValueException)
            {
                Debug.WriteLine("Salim");
            }

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/SalesIncentivePrograms/getByID")]
        public SalesIncentivePrograms GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/SalesIncentivePrograms/create")]
        public async Task<SalesIncentivePrograms> Create([FromForm]SalesIncentivePrograms model, [FromForm]IEnumerable<IFormFile> files, [FromForm]IEnumerable<IFormFile> images, [FromForm]IEnumerable<IFormFile> videos)
        {
            if (model != null)
            {
                var user = _userManager.Users.FirstOrDefault(x => x.UserName == "admin");
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
                model.CreatorUsername = "admin";
                model.LastModifierUsername = "admin";
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
            
            return model;
        }

        [HttpGet("/api/services/app/backoffice/SalesIncentivePrograms/incentiveUser")]
        public IncentiveUserVM IncentiveUser(Guid id)
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

            return incentiveUser;
        }

        [HttpPost("/api/services/app/backoffice/SalesIncentivePrograms/insertIncentiveUser")]
        public IncentiveUserVM InsertIncentiveUser([FromForm]IncentiveUserVM model)
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

            return model;
        }

        [HttpPut("/api/services/app/backoffice/SalesIncentivePrograms/update")]
        public SalesIncentivePrograms Edit(SalesIncentivePrograms model)
        {
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.UtcNow.AddHours(7);

                _appService.Update(model);
            }
            return model;
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
                attachments.CreatorUsername = "admin";
                attachments.LastModifierUsername = "admin";
                attachments.DeleterUsername = null;
                attachments.SalesIncentiveProgramId = model.Id;
                attachments.Order = order;
                attachments.Title = namaFile;
                attachments.StorageUrl = cloudBlockBlob.Uri.AbsoluteUri;
                attachments.FileName = file.FileName;
            }

            return attachments;
        }

        [HttpGet("/api/services/app/backoffice/SalesIncentivePrograms/getAttachments")]
        public List<SalesIncentiveProgramAttachments> GetAttachments(Guid guid, String attachmentType)
        {
            if (string.IsNullOrEmpty(attachmentType))
            {
                return _appService.GetById(guid).SalesIncentiveProgramAttachments.ToList().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.FileName.Contains(attachmentType)).ToList();
            }

            return _appService.GetById(guid).SalesIncentiveProgramAttachments.Where(x=> x.DeletionTime == null).ToList();
        }

        [HttpPost("/api/services/app/backoffice/SalesIncentivePrograms/uploadAttachment")]
        public SalesIncentivePrograms UploadAttachment([FromForm]Guid Id, IEnumerable<IFormFile> images, [FromForm]IEnumerable<IFormFile> documents, IEnumerable<IFormFile> videos)
        {
            var model = _appService.GetById(Id);
            IEnumerable<IFormFile> files = images.Count() > 0 ? images : videos.Count() > 0 ? videos : documents;

            if (model != null)
            {
                if (files.Count() > 0)
                {
                    var tmp = _appService.GetById(Id).SalesIncentiveProgramAttachments.Where(x => x.Title.Contains("IMG") && string.IsNullOrEmpty(x.DeleterUsername)).Count();
                    foreach (var file in files)
                    {
                        var newFile = InsertToAzure(file, model, "Edit").Result;
                        _attachmentAppService.Create(newFile);
                    }

                    model = _appService.GetById(Id);
                    model.LastModifierUsername = "admin";
                    model.LastModificationTime = DateTime.Now;

                    //Check if featuredimgurl empty
                    if (string.IsNullOrEmpty(model.FeaturedImageUrl) && images.Count() > 0)
                    {
                        model.FeaturedImageUrl = _appService.GetById(Id).SalesIncentiveProgramAttachments.FirstOrDefault(x => x.Title.Contains("IMG") && string.IsNullOrEmpty(x.DeleterUsername)).StorageUrl;
                    }
                    _appService.Update(model);
                }

            }
            
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/SalesIncentivePrograms/destroy")]
        public String Destroy(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }

        [HttpDelete("/api/services/app/backoffice/SalesIncentivePrograms/deleteAttachment")]
        public String DestroyAttachmentBackoffice(Guid item)
        {
            _attachmentAppService.SoftDelete(item, "admin");
            return "Successfully deleted";
        }

        [HttpPost("/api/services/app/backoffice/SalesIncentivePrograms/DownloadExcel")]
        public ActionResult DownloadExcel()
        {
            string rootFolder = _hostingEnvironment.WebRootPath;
            string excelName = "Sales Incentive Template.xlsx";

            var stream = new MemoryStream();

            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var workSheet = package.Workbook.Worksheets.Add("SalesIncentiveTemplate");

                    workSheet.Row(1).Height = 20;
                    workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Row(1).Style.Font.Bold = true;

                    workSheet.Cells[1, 1].Value = "Karesidenan";
                    workSheet.Cells[1, 2].Value = "Kota";
                    workSheet.Cells[1, 3].Value = "Nama Dealer";
                    workSheet.Cells[1, 4].Value = "Kode Dealer";
                    workSheet.Cells[1, 5].Value = "Tipe";
                    workSheet.Cells[1, 6].Value = "Target";

                    workSheet.Column(1).AutoFit();
                    workSheet.Column(2).AutoFit();
                    workSheet.Column(3).AutoFit();
                    workSheet.Column(4).AutoFit();
                    workSheet.Column(5).AutoFit();
                    workSheet.Column(6).AutoFit();

                    package.Save();
                }
            }
            catch (Exception ex)
            {

            }

            stream.Position = 0;

            return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = excelName
            };
        }

        [HttpPost("/api/services/app/backoffice/SalesIncentivePrograms/ImportExcel")]
        public async Task<String> ImportExcel([FromForm] IEnumerable<IFormFile> files)
        {
            if (files.Count() > 0)
            {
                var file = files.FirstOrDefault();

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets["SalesIncentiveTemplate"];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var Karesidenan = worksheet.Cells[row, 1].Value?.ToString().Trim();
                            var Kota = worksheet.Cells[row, 2].Value?.ToString().Trim();
                            var NamaDealer = worksheet.Cells[row, 3].Value?.ToString().Trim();
                            var KodeDealer = worksheet.Cells[row, 4].Value?.ToString().Trim();
                            var Tipe = worksheet.Cells[row, 5].Value?.ToString().Trim();
                            var Target = worksheet.Cells[row, 6].Value?.ToString().Trim();

                            if (Karesidenan == null && Kota == null && NamaDealer == null && KodeDealer == null && Tipe == null && Target == null) break;

                            SalesIncentiveProgramTargetCreateDto item = new SalesIncentiveProgramTargetCreateDto
                            {
                                Kota = Kota,
                                DealerId = KodeDealer,
                                DealerName = NamaDealer,
                                Karesidenan = Karesidenan,
                                EnumTipeTransaksi = Tipe,
                                Target = Convert.ToInt16(Target)
                            };
                            _incentiveTargetAppService.Create(item);
                        }
                    }
                }
            }
            return "Success Import";
        }
    }
}