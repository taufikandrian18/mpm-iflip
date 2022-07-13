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
    public class ServiceProgramManagementController : FLPAppServiceBase, IServiceProgramManagementController
    {
        private readonly UserManager _userManager;
        private readonly ServiceProgramAppService _appService;
        private readonly ServiceProgramAttachmentAppService _attachmentAppService;
        private readonly IActivityLogAppService _activityLogAppService;

        public ServiceProgramManagementController(UserManager userManager, ServiceProgramAppService appService, ServiceProgramAttachmentAppService attachmentAppService, IActivityLogAppService activityLogAppService)
        {
            _userManager = userManager;
            _appService = appService;
            _attachmentAppService = attachmentAppService;
            _activityLogAppService = activityLogAppService;
        }

        [HttpGet("/api/services/app/backoffice/ServiceProgramManagement/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _appService.GetAllBackoffice();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Title.Contains(request.Query) || x.CreatorUsername.Contains(request.Query));
            }

            var count = query.Count();

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/ServiceProgramManagement/getByID")]
        public ServicePrograms GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/ServiceProgramManagement/create")]
        public async Task<ServicePrograms> Create([FromForm]ServicePrograms model, [FromForm]IEnumerable<IFormFile> files, [FromForm]IEnumerable<IFormFile> images, [FromForm]IEnumerable<IFormFile> videos)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.UserName == "admin");
            var roles = _userManager.GetRolesAsync(user).Result.ToList();

            model.Id = Guid.NewGuid();
            model.CreationTime = DateTime.Now;
            model.CreatorUsername = "admin";
            model.LastModifierUsername = "admin";
            model.LastModificationTime = DateTime.Now;
            model.DeleterUsername = "";
            model.FeaturedImageUrl = "";

            foreach (var image in images)
            {
                model.ServiceProgramAttachments.Add(await InsertToAzure(image, model, "Create"));
                model.FeaturedImageUrl = model.ServiceProgramAttachments.Where(x => x.Title.Contains("IMG")).FirstOrDefault().StorageUrl;
            }
            foreach (var file in files)
            {
                model.ServiceProgramAttachments.Add(await InsertToAzure(file, model, "Create"));
            }
            foreach (var video in videos)
            {
                model.ServiceProgramAttachments.Add(await InsertToAzure(video, model, "Create"));
            }

            _appService.Create(model);

            return model;
        }

        private async Task<ServiceProgramAttachments> InsertToAzure(IFormFile file, ServicePrograms model, string mode)
        {
            ServiceProgramAttachments attachments = new ServiceProgramAttachments();
            
            var configuration = new AzureController().GetConnectionToAzure();

            string conn = configuration.GetConnectionString(FLPConsts.AzureConnectionString);

            CloudStorageAccount cloudStorage;
            if (CloudStorageAccount.TryParse(conn, out cloudStorage))
            {
                CloudBlobClient cloudBlobClient = cloudStorage.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("serviceprogramsmanagement");

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

                if (model.ServiceProgramAttachments.Where(x=>string.IsNullOrEmpty(x.DeleterUsername)).Count() == 0 )
                {
                    namaFile = fileType + "_" + model.Id + "_" + DateTime.Now.ToString("yyyyMMdd") + "_1" + path;
                    order = "1";
                }
                else
                {
                    if (mode == "Create")
                    {
                        order = (int.Parse(model.ServiceProgramAttachments.Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
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
                attachments.ServiceProgramId = model.Id;
                attachments.Title = namaFile;
                attachments.Order = order;
                attachments.StorageUrl = cloudBlockBlob.Uri.AbsoluteUri;
                attachments.FileName = file.FileName;
            }

            return attachments;
        }

        [HttpPut("/api/services/app/backoffice/ServiceProgramManagement/update")]
        public async Task<ServicePrograms> Edit(ServicePrograms model)
        {
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;

                _appService.Update(model);
            }
            return model;
        }

        [HttpPost("/api/services/app/backoffice/ServiceProgramManagement/uploadAttachment")]
        public ServicePrograms UploadAttachment([FromForm]Guid Id, [FromForm]IEnumerable<IFormFile> images, [FromForm]IEnumerable<IFormFile> videos, [FromForm]IEnumerable<IFormFile> documents)
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

        [HttpDelete("/api/services/app/backoffice/ServiceProgramManagement/destroy")]
        public String Destroy(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }

        [HttpGet("/api/services/app/backoffice/ServiceProgramManagement/getAttachments")]
        public List<ServiceProgramAttachments> GetAttachmentBackoffice(Guid modelId, String attachmentType)
        {
            return _appService.GetAllAttachments(modelId).Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.Title.Contains(attachmentType)).OrderBy(x => x.Title).ToList();
        }

        [HttpDelete("/api/services/app/backoffice/ServiceProgramManagement/deleteAttachment")]
        public String DestroyAttachmentBackoffice(Guid item)
        {
            _attachmentAppService.SoftDelete(item, "admin");

            return "Successfully deleted";
        }
    }
}