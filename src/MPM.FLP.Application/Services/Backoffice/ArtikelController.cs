using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using MPM.FLP.Authorization.Users;
using MPM.FLP.Authorization.Roles;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System.Diagnostics;

namespace MPM.FLP.Services.Backoffice
{
    public class ArtikelController : FLPAppServiceBase, IArtikelController
    {
        private readonly UserManager _userManager;
        private readonly ArticleAppService _appService;
        private readonly ArticleAttachmentAppService _attachmentAppService;

        public ArtikelController(ArticleAppService appService, ArticleAttachmentAppService attachmentAppService, UserManager userManager,  RoleManager roleManager)
        {
            _appService = appService;
            _attachmentAppService = attachmentAppService;
            _userManager = userManager;
        }

        [HttpGet("/api/services/app/backoffice/Artikel/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query =_appService.GetAll();

            if(!string.IsNullOrEmpty(request.Query)){
                query = query.Where(x=> x.Title.Contains(request.Query) || x.Resource.Contains(request.Query) || x.Contents.Contains(request.Query));
            }

            var count = query.Count();

            query = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit);
            var data = new List<Articles>();

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

        [HttpGet("/api/services/app/backoffice/Artikel/getByID")]
        public Articles GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/Artikel/create")]
        public async Task<Articles> CreateBackoffice([FromForm]Articles model, [FromForm]IEnumerable<IFormFile> files, [FromForm]IEnumerable<IFormFile> images, [FromForm]IEnumerable<IFormFile> videos)
        {
            if (model != null)
            {
                var user = _userManager.Users.FirstOrDefault(x => x.UserName == model.CreatorUsername);
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
                model.CreationTime = DateTime.Now;
                //model.CreatorUsername = model.CreatorUsername;
                //model.LastModifierUsername = model.LastModifierUsername;
                model.LastModificationTime = DateTime.Now;
                //model.DeleterUsername = "";
                model.ViewCount = 0;
                model.Resource = resource;


                if (images.Count() > 0)
                {
                    foreach (var image in images)
                    {
                        model.ArticleAttachments.Add(await InsertToAzure(image, model, "Create"));
                    }

                    model.FeaturedImageUrl = model.ArticleAttachments.First().StorageUrl;
                }

                foreach (var file in files)
                {
                    if (file.ContentType.Contains("application"))
                        model.ArticleAttachments.Add(await InsertToAzure(file, model, "Create"));
                }
                foreach (var video in videos)
                {
                    model.ArticleAttachments.Add(await InsertToAzure(video, model, "Create"));
                }

                _appService.Create(model);
            }
            return model;
        }

        private async Task<ArticleAttachments> InsertToAzure(IFormFile file, Articles model, string mode)
        {
            ArticleAttachments attachments = new ArticleAttachments();

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
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("article");

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
                if (model.ArticleAttachments.Count == 0)
                {
                    namaFile = fileType+"_" + model.Id + "_" + DateTime.Now.ToString("yyyyMMdd") + "_1" + path;
                    order = "1";
                }
                else
                {
                    if (mode == "Create")
                    {
                        order = (int.Parse(model.ArticleAttachments.OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
                    }
                    else
                    {
                        order = (int.Parse(_appService.GetAllAttachments(model.Id).OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
                    }


                    namaFile = fileType+"_" + model.Id + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + order + path;
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
                attachments.ArticleId = model.Id;
                attachments.Order = order;
                attachments.Title = namaFile;
                attachments.StorageUrl = cloudBlockBlob.Uri.AbsoluteUri;
                attachments.FileName = file.FileName;
            }

            return attachments;
        }

        [HttpPut("/api/services/app/backoffice/Artikel/update")]
        public Articles UpdateBackoffice(Articles model)
        {
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;

                _appService.Update(model);
            }
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/Artikel/destroy")]
        public String DestroyBackoffice(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }

        [HttpGet("/api/services/app/backoffice/Artikel/getAttachments")]
        public List<ArticleAttachments> GetAttachmentBackoffice(Guid modelId, String attachmentType)
        {
            if (!string.IsNullOrEmpty(attachmentType))
                return _appService.GetAllAttachments(modelId).Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.Title.Contains(attachmentType)).OrderBy(x => x.Title).ToList();
            return _appService.GetAllAttachments(modelId).Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderBy(x => x.Title).ToList();
        }

        public async Task<ArticleAttachments> UpdateAttachmentsAzure(string fileURL, string fileType, Articles model)
        {
            AzureController azureController = new AzureController();

            string namaFile = "";
            string order = "";

            if (model.ArticleAttachments.Count == 0)
            {
                order = "1";
                namaFile = fileType + "_" + model.Id + "_" + DateTime.Now.ToString("yyyyMMdd") + "_1";
            }
            else
            {   
                order = (int.Parse(_appService.GetAllAttachments(model.Id).OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
                namaFile = fileType + "_" + model.Id + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + order;
            }

            ArticleAttachments attachments = new ArticleAttachments();
            attachments.Id = Guid.NewGuid();
            attachments.CreationTime = DateTime.Now;
            attachments.CreatorUsername = "admin";
            attachments.LastModifierUsername = "admin";
            attachments.DeleterUsername = null;
            attachments.ArticleId = model.Id;
            attachments.Order = order;
            attachments.Title = namaFile;
            attachments.StorageUrl = fileURL;
            attachments.FileName = namaFile;

            return attachments;
        }

        [HttpPut("/api/services/app/backoffice/Artikel/updateAttachment")]
        public async Task<Articles> UpdateAttachmentBackoffice([FromForm]Guid Id, [FromForm]List<string> files, [FromForm]List<string> images, [FromForm]List<string> videos)
        {
            var model = _appService.GetById(Id);
            AzureController azureController = new AzureController();
            if (model != null)
            {
                foreach (var file in files)
                {
                    _attachmentAppService.Create(await UpdateAttachmentsAzure(file, "DOC", model));
                }
                
                foreach (var image in images)
                {
                    var imageObject = await UpdateAttachmentsAzure(image, "IMG", model);
                    _attachmentAppService.Create(imageObject);
                }

                foreach (var video in videos)
                {
                    /*
                    var newFile = InsertToAzure(video, model, "Edit").Result;
                    _attachmentAppService.Create(newFile);
                    */
                    _attachmentAppService.Create(await UpdateAttachmentsAzure(video, "VID", model));
                }

                model = _appService.GetById(Id);
                //Check if featuredimgurl empty
                if (string.IsNullOrEmpty(model.FeaturedImageUrl))
                {
                    model.FeaturedImageUrl = _appService.GetAllAttachments(Id).FirstOrDefault(x => string.IsNullOrEmpty(x.DeleterUsername)).StorageUrl;
                }

                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;
                _appService.Update(model);

                return model;
            }

            return null;
        }

        [HttpDelete("/api/services/app/backoffice/Artikel/deleteAttachment")]
        public String DestroyAttachmentBackoffice(Guid id)
        {
            _attachmentAppService.SoftDelete(id, "admin");

            return "Successfully deleted";
        }
    }
}
