using Microsoft.AspNetCore.Mvc;
using MPM.FLP.Services;
using System.Collections.Generic;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using MPM.FLP.Authorization.Users;

namespace MPM.FLP.Services.Backoffice
{
    public class InfoMainDealersController : FLPAppServiceBase, IInfoMainDealersController
    {
        private readonly UserManager _userManager;
        private readonly InfoMainDealerAppService _appService;
        private readonly InfoMainDealerAttachmentAppService _attachmentAppService;
        private readonly IActivityLogAppService _activityLogAppService;

        public InfoMainDealersController(UserManager userManager, InfoMainDealerAppService appService, InfoMainDealerAttachmentAppService attachmentAppService, IActivityLogAppService activityLogAppService)
        {
            _userManager = userManager;
            _appService = appService;
            _attachmentAppService = attachmentAppService;
            _activityLogAppService = activityLogAppService;
        }

        [HttpGet("/api/services/app/backoffice/InfoMainDealers/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);
            var query = _appService.GetAll().Where(x=> x.DeletionTime ==null);

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Title.Contains(request.Query) || x.Contents.Contains(request.Query) || x.CreatorUsername.Contains(request.Query));
            }
            var count = query.Count();
            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/InfoMainDealers/getByID")]
        public InfoMainDealers GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/InfoMainDealers/create")]
        public async Task<InfoMainDealers> Create([FromForm]InfoMainDealers model, [FromForm]IEnumerable<IFormFile> files, [FromForm]IEnumerable<IFormFile> images, [FromForm]IEnumerable<IFormFile> videos)
        {
            if (model != null)
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
                    model.InfoMainDealerAttachments.Add(await InsertToAzure(image, model, "Create"));
                    model.FeaturedImageUrl = model.InfoMainDealerAttachments.FirstOrDefault(x => x.Title.Contains("IMG") && string.IsNullOrEmpty(x.DeleterUsername)).StorageUrl;
                }
                foreach (var file in files)
                {
                    model.InfoMainDealerAttachments.Add(await InsertToAzure(file, model, "Create"));
                }
                foreach (var video in videos)
                {
                    model.InfoMainDealerAttachments.Add(await InsertToAzure(video, model, "Create"));
                }

                _appService.Create(model);

            };

            return model;
        }

        private async Task<InfoMainDealerAttachments> InsertToAzure(IFormFile file, InfoMainDealers model, string mode)
        {
            InfoMainDealerAttachments attachments = new InfoMainDealerAttachments();

            var configuration = new AzureController().GetConnectionToAzure();
            //var configuration = AppConfigurations.Get(AppDomain.CurrentDomain.BaseDirectory);

            string conn = configuration.GetConnectionString(FLPConsts.AzureConnectionString);

            CloudStorageAccount cloudStorage;
            if (CloudStorageAccount.TryParse(conn, out cloudStorage))
            {
                CloudBlobClient cloudBlobClient = cloudStorage.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("infomaindealer");

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
                if (model.InfoMainDealerAttachments.Count == 0)
                {
                    namaFile = fileType + "_" + model.Id + "_" + DateTime.Now.ToString("yyyyMMdd") + "_1" + path;
                    order = "1";
                }
                else
                {
                    if (mode == "Create")
                    {
                        order = (int.Parse(model.InfoMainDealerAttachments.OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
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
                attachments.CreatorUsername = "admin";
                attachments.LastModifierUsername = "admin";
                attachments.DeleterUsername = null;
                attachments.InfoMainDealerId = model.Id;
                attachments.Order = order;
                attachments.Title = namaFile;
                attachments.StorageUrl = cloudBlockBlob.Uri.AbsoluteUri;
                attachments.FileName = file.FileName;
            }

            return attachments;
        }

        [HttpPut("/api/services/app/backoffice/InfoMainDealers/update")]
        public InfoMainDealers Edit(InfoMainDealers model)
        {
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;

                _appService.Update(model);
            }
            return model;
        }

        [HttpPost("/api/services/app/backoffice/InfoMainDealers/destroy")]
        public String Destroy([FromForm]Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }

        [HttpPost("/api/services/app/backoffice/InfoMainDealers/upload_attachment")]
        public InfoMainDealers UploadAttachment([FromForm]Guid Id, [FromForm]IEnumerable<IFormFile> images, [FromForm]IEnumerable<IFormFile> documents, [FromForm]IEnumerable<IFormFile> videos)
        {
            var model = _appService.GetById(Id);
            IEnumerable<IFormFile> files = images.Count() > 0 ? images : videos.Count() > 0 ? videos : documents;

            if (model != null)
            {
                if (files.Count() > 0)
                {
                    var tmp = _appService.GetById(Id).InfoMainDealerAttachments.Where(x => x.Title.Contains("IMG") && string.IsNullOrEmpty(x.DeleterUsername)).Count();

                    foreach (var file in files)
                    {
                        //model.GuideAttachments.Add(await InsertToAzure(file, model, "Edit"));
                        var newFile = InsertToAzure(file, model, "Edit").Result;

                        _attachmentAppService.Create(newFile);
                    }

                    model = _appService.GetById(Id);
                    model.LastModifierUsername = "admin";
                    model.LastModificationTime = DateTime.Now;

                    //Check if featuredimgurl empty
                    if (string.IsNullOrEmpty(model.FeaturedImageUrl) && images.Count() > 0)
                    {
                        model.FeaturedImageUrl = _appService.GetAllAttachments(Id).FirstOrDefault(x => x.Title.Contains("IMG") && string.IsNullOrEmpty(x.DeleterUsername)).StorageUrl;
                    }
                    _appService.Update(model);
                }

            }


            return model;
        }

        [HttpGet("/api/services/app/backoffice/InfoMainDealers/getAllAttachment")]
        public List<InfoMainDealerAttachments> GetAttachmentBackoffice(Guid modelId)
        {
            return _appService.GetAllAttachments(modelId).Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderBy(x => x.Title).ToList();
        }

        [HttpDelete("/api/services/app/backoffice/InfoMainDealers/destroyAttachment")]
        public String DestroyAttachment(Guid guid)
        {
            _attachmentAppService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }
    }
}
