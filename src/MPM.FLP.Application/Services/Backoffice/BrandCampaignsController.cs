using Microsoft.AspNetCore.Mvc;
using Abp.AspNetCore.Mvc.Authorization;
using MPM.FLP.Services;
using System.Collections.Generic;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Web;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using MPM.FLP.Services.Backoffice;

namespace MPM.FLP.Web.Mvc.Controllers
{
    public class BrandCampaignsController : FLPAppServiceBase, IBrandCampaignsController
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

        [HttpGet("/api/services/app/backoffice/BrandCampaigns/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query =_appService.GetAll();

            if(!string.IsNullOrEmpty(request.Query)){
                query = query.Where(x=> x.Title.Contains(request.Query) || x.Contents.Contains(request.Query) || x.CreatorUsername.Contains(request.Query));
            }

            var count = query.Count();

            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/BrandCampaigns/getByID")]
        public BrandCampaigns GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/BrandCampaigns/create")]
        public async Task<BrandCampaigns> CreateBackoffice([FromForm]BrandCampaigns model, [FromForm]IEnumerable<IFormFile> images)
        {
            if (model != null)
            {
                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.Now;
                model.CreatorUsername = "admin";
                model.LastModifierUsername = "admin";
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
            return model;
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
                attachments.CreatorUsername = "admin";
                attachments.LastModifierUsername = "admin";
                attachments.DeleterUsername = null;
                attachments.BrandCampaignId = model.Id;
                attachments.Order = order;
                attachments.Title = namaFile;
                attachments.StorageUrl = cloudBlockBlob.Uri.AbsoluteUri;
                attachments.FileName = file.FileName;
            }

            return attachments;
        }

        [HttpPut("/api/services/app/backoffice/BrandCampaigns/update")]
        public BrandCampaigns UpdateBackoffice(BrandCampaigns model)
        {
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;

                _appService.Update(model);
            }
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/BrandCampaigns/destroy")]
        public String DestroyBackoffice(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }

        [HttpGet("/api/services/app/backoffice/BrandCampaigns/getAttachments")]
        public List<BrandCampaignAttachments> GetAttachmentBackoffice(Guid modelId)
        {
            return _appService.GetAllAttachments(modelId).Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderBy(x => x.Title).ToList();
        }

        [HttpPut("/api/services/app/backoffice/BrandCampaigns/updateAttachments")]
        public BrandCampaigns UpdateAttachmentBackoffice(Guid Id, IEnumerable<IFormFile> files)
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
                    model.LastModifierUsername = "admin";
                    model.LastModificationTime = DateTime.Now;
                    if (model.FeaturedImageUrl == "" || model.FeaturedImageUrl == null)
                    {
                        model.FeaturedImageUrl = model.BrandCampaignAttachments.FirstOrDefault(x => string.IsNullOrEmpty(x.DeleterUsername)).StorageUrl;
                    }
                    _appService.Update(model);
                }
            }
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/BrandCampaigns/destroyAttachments")]
        public String DestroyAttachmentBackoffice(Guid id)
        {
            _attachmentAppService.SoftDelete(id, "admin");
            return "Successfully deleted";
        }
    }
}
