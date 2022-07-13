using Microsoft.AspNetCore.Mvc;
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
using MPM.FLP.Authorization.Users;

namespace MPM.FLP.Services.Backoffice
{
    public class ManagementPanduanTeknikalController : FLPAppServiceBase, IManagementPanduanTeknikalController
    {
        private readonly UserManager _userManager;
        private readonly GuideAppService _appService;
        private readonly GuideCategoryAppService _categoryAppService;
        private readonly GuideAttachmentAppService _attachmentAppService;

        public ManagementPanduanTeknikalController(UserManager userManager, GuideAppService appService, GuideCategoryAppService categoryAppService, GuideAttachmentAppService attachmentAppService)
        {
            _userManager = userManager;
            _appService = appService;
            _categoryAppService = categoryAppService;
            _attachmentAppService = attachmentAppService;
        }

        [HttpGet("/api/services/app/backoffice/ManagementPanduanTeknikalController/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _appService.GetAll().Where(x => x.IsTechnicalGuide);

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => 
                    x.Title.Contains(request.Query) || 
                    x.Contents.Contains(request.Query) || 
                    x.GuideCategoryId.ToString() == request.Query || 
                    x.CreatorUsername.Contains(request.Query)
                );
            }

            var count = query.Count();

            //var data = query.OrderBy(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();
            var data = query.OrderByDescending(x => x.CreationTime)
                .ThenBy(x => x.Title)        
                .Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/ManagementPanduanTeknikalController/getByID")]
        public Guides GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpGet("/api/services/app/backoffice/ManagementPanduanTeknikalController/getGuideCategories")]
        public List<GuideCategories> GetGuideCategories()
        {
            IEnumerable<GuideCategories> guideCategories = _categoryAppService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername));
            return guideCategories.ToList();
        }

        [HttpPost("/api/services/app/backoffice/ManagementPanduanTeknikalController/create")]
        public async Task<Guides> Create([FromForm]Guides model, [FromForm]IEnumerable<IFormFile> files, [FromForm]IEnumerable<IFormFile> images, [FromForm]IEnumerable<IFormFile> videos)
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
                else {
                    resource = "";
                }

                model.Id = Guid.NewGuid();
                model.IsTechnicalGuide = true;
                model.CreationTime = DateTime.Now;
                model.CreatorUsername = "admin";
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;
                model.DeleterUsername = "";
                model.FeaturedImageUrl = "";
                model.Resource = resource;

                foreach (var image in images)
                {
                    model.GuideAttachments.Add(await InsertToAzure(image, model, "Create"));
                    model.FeaturedImageUrl = model.GuideAttachments.FirstOrDefault(x => x.Title.Contains("IMG") && string.IsNullOrEmpty(x.DeleterUsername)).StorageUrl;
                }
                foreach (var file in files)
                {
                    model.GuideAttachments.Add(await InsertToAzure(file, model, "Create"));
                }
                foreach (var video in videos)
                {
                    model.GuideAttachments.Add(await InsertToAzure(video, model, "Create"));
                }

                _appService.Create(model, "Panduan Teknikal");
            };

            return model;
        }

        private async Task<GuideAttachments> InsertToAzure(IFormFile file, Guides model, string mode)
        {
            GuideAttachments attachments = new GuideAttachments();

            var configuration = new AzureController().GetConnectionToAzure();
            //var configuration = AppConfigurations.Get(AppDomain.CurrentDomain.BaseDirectory);

            string conn = configuration.GetConnectionString(FLPConsts.AzureConnectionString);

            CloudStorageAccount cloudStorage;
            if (CloudStorageAccount.TryParse(conn, out cloudStorage))
            {
                CloudBlobClient cloudBlobClient = cloudStorage.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("panduanteknikal");

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
                if (model.GuideAttachments.Count == 0)
                {
                    namaFile = fileType + "_" + model.Id + "_" + DateTime.Now.ToString("yyyyMMdd") + "_1" + path;
                    order = "1";
                }
                else
                {
                    if (mode == "Create")
                    {
                        order = (int.Parse(model.GuideAttachments.OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
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
                attachments.GuideId = model.Id;
                attachments.Order = order;
                attachments.Title = namaFile;
                attachments.StorageUrl = cloudBlockBlob.Uri.AbsoluteUri;
                attachments.FileName = file.FileName;
            }

            return attachments;
        }

        [HttpPost("/api/services/app/backoffice/ManagementPanduanTeknikalController/update")]
        public Guides Edit(Guides model)
        {
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;

                _appService.Update(model, "Panduan Teknikal");
            }
            return model;
        }

        [HttpPost("/api/services/app/backoffice/ManagementPanduanTeknikalController/uploadAttachment")]
        public String UploadAttachment([FromForm]Guid Id, [FromForm]IEnumerable<IFormFile> images, [FromForm]IEnumerable<IFormFile> documents, [FromForm]IEnumerable<IFormFile> videos)
        {
            var model = _appService.GetById(Id);
            IEnumerable<IFormFile> files = images.Count() > 0 ? images : videos.Count() > 0 ? videos : documents;

            if (model != null)
            {
                if (files.Count() > 0)
                {
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
                    _appService.Update(model, "Panduan Teknikal");
                }
                
            }

            return "Success";
        }

        [HttpDelete("/api/services/app/backoffice/ManagementPanduanTeknikalController/destroy")]
        public String Destroy(Guid guid)
        {
            _appService.SoftDelete(guid, "admin", "Panduan Teknikal");
            return "Successfully deleted";
        }

        [HttpGet("/api/services/app/backoffice/ManagementPanduanTeknikalController/getAttachments")]
        public List<GuideAttachments> GetAttachmentBackoffice(Guid modelId, String attachmentType)
        {
            if (!string.IsNullOrEmpty(attachmentType))
                return _appService.GetAllAttachments(modelId).Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.Title.Contains(attachmentType)).OrderBy(x => x.Title).ToList();

            return _appService.GetAllAttachments(modelId).Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderBy(x => x.Title).ToList();
        }

        [HttpDelete("/api/services/app/backoffice/ManagementPanduanTeknikalController/deleteAttachment")]
        public String DestroyAttachmentBackoffice(Guid item)
        {
            _attachmentAppService.SoftDelete(item, "admin");
            return "Successfully deleted";
        }

        Guides IManagementPanduanTeknikalController.UploadAttachment(Guid Id, IEnumerable<IFormFile> images, IEnumerable<IFormFile> documents, IEnumerable<IFormFile> videos)
        {
            throw new NotImplementedException();
        }
    }
}