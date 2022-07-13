using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using MPM.FLP.Authorization.Users;
using MPM.FLP.Services.Dto;

namespace MPM.FLP.Services.Backoffice
{
    public class CSChampionClubsController : FLPAppServiceBase, ICSChampionClubsController
    {
        private readonly UserManager _userManager;
        private readonly CSChampionClubAppService _appService;
        private readonly CSChampionClubParticipantAppService _participantAppService;
        private readonly CSChampionClubAttachmentAppService _attachmentAppService;
        private readonly CSChampionClubRegistrationAppService _regisAppService;
        private readonly InternalUserAppService _userAppService;

        public CSChampionClubsController(UserManager userManager, CSChampionClubAppService appService, CSChampionClubParticipantAppService participantAppService, CSChampionClubAttachmentAppService attachmentAppService, CSChampionClubRegistrationAppService regisAppService, InternalUserAppService userAppService)
        {
            _userManager = userManager;
            _appService = appService;
            _participantAppService = participantAppService;
            _attachmentAppService = attachmentAppService;
            _regisAppService = regisAppService;
            _userAppService = userAppService;
        }

        #region Article
        [HttpGet("/api/services/app/backoffice/CSChampionClubs/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);
            var query = _appService.GetAll();

            if(!string.IsNullOrEmpty(request.Query)){
                query = query.Where(x=> x.Title.Contains(request.Query) || x.CreatorUsername.Contains(request.Query) || x.Contents.Contains(request.Query));
            }
            var count = query.Count();
            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/CSChampionClubs/getByID")]
        public CSChampionClubs GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/CSChampionClubs/create")]
        public async Task<CSChampionClubs> Create([FromForm]CSChampionClubsVM data, [FromForm]IEnumerable<IFormFile> files, [FromForm]IEnumerable<IFormFile> images, [FromForm]IEnumerable<IFormFile> videos)
        {
            CSChampionClubs model = new CSChampionClubs();

            model.Id = Guid.NewGuid();
            model.CreationTime = DateTime.Now;
            model.CreatorUsername = "admin";
            model.LastModifierUsername = "admin";
            model.LastModificationTime = DateTime.Now;
            model.DeleterUsername = "";
            model.FeaturedImageUrl = "";
            model.IsRegistered = false;
            model.IsRegistrationCanceled = false;
            model.Title = data.Title;
            model.Contents = data.Contents;
            model.ReadingTime = data.ReadingTime;
            model.ViewCount = data.ViewCount;
            model.IsPublished = data.IsPublished;

            foreach (var image in images)
            {
                model.CSChampionClubAttachments.Add(await InsertToAzure(image, model, "Create"));
                model.FeaturedImageUrl = model.CSChampionClubAttachments.FirstOrDefault(x => x.Title.Contains("IMG") && string.IsNullOrEmpty(x.DeleterUsername)).StorageUrl;
            }
            foreach (var file in files)
            {
                model.CSChampionClubAttachments.Add(await InsertToAzure(file, model, "Create"));
            }
            foreach (var video in videos)
            {
                model.CSChampionClubAttachments.Add(await InsertToAzure(video, model, "Create"));
            }

            _appService.Create(model);

            return model;
        }

        private async Task<CSChampionClubAttachments> InsertToAzure(IFormFile file, CSChampionClubs model, string mode)
        {
            CSChampionClubAttachments attachments = new CSChampionClubAttachments();

            var configuration = new AzureController().GetConnectionToAzure();
            //var configuration = AppConfigurations.Get(AppDomain.CurrentDomain.BaseDirectory);

            string conn = configuration.GetConnectionString(FLPConsts.AzureConnectionString);

            CloudStorageAccount cloudStorage;
            if (CloudStorageAccount.TryParse(conn, out cloudStorage))
            {
                CloudBlobClient cloudBlobClient = cloudStorage.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("cschampionclubs");

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
                if (model.CSChampionClubAttachments.Count == 0)
                {
                    namaFile = fileType + "_" + model.Id + "_" + DateTime.Now.ToString("yyyyMMdd") + "_1" + path;
                    order = "1";
                }
                else
                {
                    if (mode == "Create")
                    {
                        order = (int.Parse(model.CSChampionClubAttachments.OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
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
                attachments.CSChampionClubId = model.Id;
                attachments.Order = order;
                attachments.Title = namaFile;
                attachments.StorageUrl = cloudBlockBlob.Uri.AbsoluteUri;
                attachments.FileName = file.FileName;
            }

            return attachments;
        }

        [HttpPut("/api/services/app/backoffice/CSChampionClubs/update")]
        public CSChampionClubs EditBackoffice([FromForm]CSChampionClubsVM data)
        {
            CSChampionClubs model = _appService.GetById(data.Id);
            model.Title = data.Title;
            model.Contents = data.Contents;
            model.ReadingTime = data.ReadingTime;
            model.ViewCount = data.ViewCount;
            model.IsPublished = data.IsPublished;

            model.LastModifierUsername = "admin";
            model.LastModificationTime = DateTime.Now;

            _appService.Update(model);

            model = _appService.GetById(data.Id);
            return model;
        }

        [HttpGet("/api/services/app/backoffice/CSChampionClubs/getAttachments")]
        public List<CSChampionClubAttachments> GetAttachmentBackoffice(Guid modelId, String attachmentType)
        {
            if (!string.IsNullOrEmpty(attachmentType))
                return _appService.GetAllAttachments(modelId).Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.Title.Contains(attachmentType)).OrderBy(x => x.Title).ToList();
            return _appService.GetAllAttachments(modelId).Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderBy(x => x.Title).ToList();
        }

        [HttpPut("/api/services/app/backoffice/CSChampionClubs/updateAttachment")]
        public CSChampionClubs UploadAttachment(Guid Id, IEnumerable<IFormFile> images, IEnumerable<IFormFile> documents, IEnumerable<IFormFile> videos)
        {
            var model = _appService.GetById(Id);
            IEnumerable<IFormFile> files = images.Count() > 0 ? images : videos.Count() > 0 ? videos : documents;

            if (model != null)
            {
                if (files.Count() > 0)
                {
                    var tmp = _appService.GetById(Id).CSChampionClubAttachments.Where(x => x.Title.Contains("IMG") && string.IsNullOrEmpty(x.DeleterUsername)).Count();
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

        [HttpDelete("/api/services/app/backoffice/CSChampionClubs/destroy")]
        public String DestroyBackoffice(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }
        #endregion

        #region Participants
        [HttpGet("/api/services/app/backoffice/CSChampionClubs/searchParticipants")]
        public List<CSChampionClubParticipantsVM> SearchParticipants(int year)
        {
            List<CSChampionClubParticipantsVM> result = new List<CSChampionClubParticipantsVM>();

            var tmpResults = _participantAppService.GetAll().Where(x => x.Year == year).ToList();

            foreach (var tmpResult in tmpResults)
            {
                CSChampionClubParticipantsVM tmp = new CSChampionClubParticipantsVM();

                tmp.IDMPM = tmpResult.IDMPM;
                tmp.Name = _userAppService.GetAll().SingleOrDefault(x => x.IDMPM == tmp.IDMPM).Nama;
                tmp.DealerName = _userAppService.GetAll().SingleOrDefault(x => x.IDMPM == tmp.IDMPM).DealerName;

                result.Add(tmp);
            }

            return result;
        }

        [HttpGet("/api/services/app/backoffice/CSChampionClubs/getAllParticipants")]
        public BaseResponse GetAllParticipants([FromQuery]Pagination request)
        {
            request = Paginate.Validate(request);
            var query = _participantAppService.GetAll();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.IDMPM.ToString() == request.Query || x.CreatorUsername.Contains(request.Query) || x.Year.ToString() == request.Query);
            }
            var count = query.Count();
            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpPost("/api/services/app/backoffice/CSChampionClubs/updateRegistration")]
        public CSChampionClubRegistrations UpdateRegistration(CSChampionClubRegistrations model)
        {
            var modelOld = _regisAppService.GetAll().SingleOrDefault(x => x.Year == model.Year);
            if (modelOld == null)
            {
                model.CreationTime = DateTime.UtcNow.AddHours(7);
                model.CreatorUsername = "admin";
                model.LastModificationTime = DateTime.UtcNow.AddHours(7);
                model.LastModifierUsername = "admin";

                _regisAppService.Create(model);
            }
            else
            {
                modelOld.StartDate = model.StartDate;
                model.EndDate = model.EndDate;
                model.LastModificationTime = DateTime.UtcNow.AddHours(7);
                model.LastModifierUsername = "admin";

                _regisAppService.Update(modelOld);
            }
            return modelOld;
        }
        #endregion

        [HttpDelete("/api/services/app/backoffice/CSChampionClubs/deleteAttachment")]
        public String DestroyAttachmentBackoffice(Guid guid)
        {
            _attachmentAppService.SoftDelete(guid, "admin");

            return "Successfully deleted";
        }
    }
}