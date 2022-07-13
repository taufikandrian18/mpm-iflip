using Microsoft.AspNetCore.Mvc;
using MPM.FLP.Services;
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
using System.Web;
using MPM.FLP.Services.Backoffice;

namespace MPM.FLP.Services.Backoffice
{
    public class SalesTalksController : FLPAppServiceBase, ISalesTalksController
    {
        private readonly SalesTalkAppService _appService;
        private readonly SalesTalkAttachmentAppService _attachmentAppService;
        private readonly IActivityLogAppService _activityLogAppService;

        public SalesTalksController(SalesTalkAppService appService, SalesTalkAttachmentAppService attachmentAppService, IActivityLogAppService activityLogAppService)
        {
            _appService = appService;
            _attachmentAppService = attachmentAppService;
            _activityLogAppService = activityLogAppService;
        }

        [HttpGet("/api/services/app/backoffice/SalesTalks/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _appService.GetAll();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Title.Contains(request.Query) || x.Contents.Contains(request.Query) || x.CreatorUsername.Contains(request.Query));
            }

            var count = query.Count();

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/SalesTalks/getByID")]
        public SalesTalks GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/SalesTalks/create")]
        public async Task<SalesTalks> Create([FromForm]SalesTalks model, [FromForm]IEnumerable<IFormFile> files, [FromForm]IEnumerable<IFormFile> documents)
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
                if (files.Count() > 0)
                {
                    foreach (var file in files)
                    {
                        model.SalesTalkAttachments.Add(await InsertToAzure(file, model));
                    }

                    model.FeaturedImageUrl = model.SalesTalkAttachments.First().StorageUrl;
                }

                if (documents.Count() > 0)
                {
                    foreach (var file in documents)
                    {
                        model.SalesTalkAttachments.Add(await InsertToAzure(file, model));
                    }
                }
                _appService.Create(model);
            };

            return model;
        }

        private async Task<SalesTalkAttachments> InsertToAzure(IFormFile file, SalesTalks model)
        {
            SalesTalkAttachments attachments = new SalesTalkAttachments();

            var configuration = new AzureController().GetConnectionToAzure();

            string conn = configuration.GetConnectionString(FLPConsts.AzureConnectionString);

            CloudStorageAccount cloudStorage;
            if (CloudStorageAccount.TryParse(conn, out cloudStorage))
            {
                CloudBlobClient cloudBlobClient = cloudStorage.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("salestalk");

                string namaFile = "";
                string order = "";
                string fileType = "";

                if (file.ContentType.Contains("image"))
                    fileType = "IMG";
                else if (file.ContentType.Contains("application"))
                    fileType = "DOC";

                var path = Path.GetExtension(file.FileName);
                if (model.SalesTalkAttachments.Count == 0)
                {
                    namaFile = fileType + "_" + model.Id + "_" + DateTime.Now.ToString("yyyyMMdd") + "_1" + path;
                    order = "1";
                }
                else
                {
                    order = (int.Parse(model.SalesTalkAttachments.OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();

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
                attachments.SalesTalkId = model.Id;
                attachments.Title = namaFile;
                attachments.Order = order;
                attachments.StorageUrl = cloudBlockBlob.Uri.AbsoluteUri;
                attachments.FileName = file.FileName;
            }

            return attachments;
        }

        [HttpPut("/api/services/app/backoffice/SalesTalks/update")]
        public async Task<SalesTalks> Edit(SalesTalks model)
        {
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;

                if(model.FeaturedImageUrl == null)
                {
                    model.FeaturedImageUrl = "";
                }

                _appService.Update(model);
            }
            return model;
        }

        [HttpPut("/api/services/app/backoffice/SalesTalks/updateAttachment")]
        public SalesTalks EditAttachment([FromForm]Guid Id, [FromForm]IEnumerable<IFormFile> files, [FromForm]IEnumerable<IFormFile> documents)
        {
            var model = _appService.GetById(Id);

            if (model != null)
            {
                if (files.Count() > 0)
                {
                    foreach (var file in files)
                    {
                        var newFile = InsertToAzure(file, model).Result;

                        _attachmentAppService.Create(newFile);
                    }

                    model = _appService.GetById(Id);
                    model.LastModifierUsername = "admin";
                    model.LastModificationTime = DateTime.Now;
                    //Check if deleted is featuredimgurl
                    if (string.IsNullOrEmpty(model.FeaturedImageUrl))
                    {
                        model.FeaturedImageUrl = _appService.GetAllAttachments(Id).FirstOrDefault(x => string.IsNullOrEmpty(x.DeleterUsername)).StorageUrl;
                    }
                    _appService.Update(model);
                }
                if (documents.Count() > 0)
                {
                    foreach (var file in documents)
                    {
                        //model.ServiceTalkFlyerAttachments.Add(await InsertToAzure(file, model, "Edit"));
                        var newFile = InsertToAzure(file, model).Result;

                        _attachmentAppService.Create(newFile);
                    }

                }
            }
            return model;
        }


        [HttpDelete("/api/services/app/backoffice/SalesTalks/destroy")]
        public String Destroy(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }

        [HttpGet("/api/services/app/backoffice/SalesTalks/getAttachments")]
        public List<SalesTalkAttachments> GetAttachments(Guid guid, String attachmentType)
        {
            if (!string.IsNullOrEmpty(attachmentType))
            {
                return _appService.GetAllAttachments(guid).ToList().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.Title.Contains(attachmentType)).ToList();
            }

            return _appService.GetAllAttachments(guid).Where(x => string.IsNullOrEmpty(x.DeleterUsername)).ToList();
        }

        [HttpDelete("/api/services/app/backoffice/SalesTalks/deleteAttachment")]
        public String DestroyAttachment(Guid item)
        {
            _attachmentAppService.SoftDelete(item, "admin");
            return "Successfully deleted";
        }
    }
}