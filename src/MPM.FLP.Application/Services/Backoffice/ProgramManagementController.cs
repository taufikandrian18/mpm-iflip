using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MPM.FLP.Authorization.Users;

namespace MPM.FLP.Services.Backoffice
{
    public class ProgramManagementController : FLPAppServiceBase, IProgramManagementController
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

        [HttpGet("/api/services/app/backoffice/ProgramManagement/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _appService.GetAllBackoffice();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Title.Contains(request.Query) || x.Contents.Contains(request.Query) || x.CreatorUsername.Contains(request.Query));
            }

            var count = query.Count();

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/ProgramManagement/getByID")]
        public SalesPrograms GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/ProgramManagement/create")]
        public async Task<SalesPrograms> Create([FromForm]SalesPrograms model, [FromForm]IEnumerable<IFormFile> files, [FromForm]IEnumerable<IFormFile> images, [FromForm]IEnumerable<IFormFile> videos)
        {
            model.Id = Guid.NewGuid();
            model.CreationTime = DateTime.Now;
            model.CreatorUsername = "admin";
            model.LastModifierUsername = "admin";
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
            return model;
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
                attachments.CreatorUsername = "admin";
                attachments.LastModifierUsername = "admin";
                attachments.DeleterUsername = null;
                attachments.SalesProgramId = model.Id;
                attachments.Title = namaFile;
                attachments.Order = order;
                attachments.StorageUrl = cloudBlockBlob.Uri.AbsoluteUri;
                attachments.FileName = file.FileName;
            }

            return attachments;
        }

        [HttpPut("/api/services/app/backoffice/ProgramManagement/update")]
        public async Task<SalesPrograms> Edit(SalesPrograms model)
        {
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;

                _appService.Update(model);
            }

            return model;
        }

        [HttpPost("/api/services/app/backoffice/ProgramManagement/uploadAttachment")]
        public SalesPrograms UploadAttachment(Guid Id, IEnumerable<IFormFile> images, IEnumerable<IFormFile> videos, IEnumerable<IFormFile> documents)
        {
            var model = _appService.GetById(Id);
            IEnumerable<IFormFile> files = images.Count() > 0 ? images : videos.Count() > 0 ? videos : documents;

            if (model != null)
            {
                if (files.Count() > 0)
                {
                    foreach (var file in files)
                    {
                        var newFile = InsertToAzure(file, model, "Edit").Result;

                        _attachmentAppService.Create(newFile);
                    }

                    model = _appService.GetById(Id);
                    //Check if featuredimgurl is empty
                    if (string.IsNullOrEmpty(model.FeaturedImageUrl) && images.Count() > 0)
                    {
                        model.FeaturedImageUrl = _appService.GetAllAttachments(Id).FirstOrDefault(x => x.Title.Contains("IMG") && string.IsNullOrEmpty(x.DeleterUsername)).StorageUrl;
                    }
                    model.LastModifierUsername = "admin";
                    model.LastModificationTime = DateTime.Now;

                    _appService.Update(model);
                }
               
            }
            
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/ProgramManagement/destroy")]
        public String Destroy(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }

        [HttpGet("/api/services/app/backoffice/ProgramManagement/getAttachments")]
        public List<SalesProgramAttachments> GetAttachmentBackoffice(Guid modelId, String attachmentType)
        {
            if (!string.IsNullOrEmpty(attachmentType))
                return _appService.GetAllAttachments(modelId).Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.Title.Contains(attachmentType)).OrderBy(x => x.Title).ToList();
            return _appService.GetAllAttachments(modelId).Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderBy(x => x.Title).ToList();
        }

        [HttpDelete("/api/services/app/backoffice/ProgramManagement/destroyAttachment")]
        public String DestroyAttachment(Guid id)
        {
            _attachmentAppService.SoftDelete(id, "admin");
            return "Successfully deleted";
        }
    }
}