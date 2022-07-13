using System.Collections.Generic;
using MPM.FLP.FLPDb;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.Services.Dto;

namespace MPM.FLP.Services.Backoffice
{
    public class ClaimProgramsController : FLPAppServiceBase, IClaimProgramsController
    {
        private readonly ClaimProgramAppService _appService;
        private readonly ClaimProgramAttachmentAppService _attachmentAppService;
        private readonly ClaimProgramClaimerAppService _claimerAppService;

        public ClaimProgramsController(ClaimProgramAppService appService, ClaimProgramClaimerAppService claimerAppService, ClaimProgramAttachmentAppService attachmentAppService)
        {
            _appService = appService;
            _claimerAppService = claimerAppService;
            _attachmentAppService = attachmentAppService;
        }

        #region Main

        [HttpGet("/api/services/app/backoffice/ClaimPrograms/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);
            var query = _appService.GetAll();

            if(!string.IsNullOrEmpty(request.Query)) {
                query = query.Where(x => x.Title.Contains(request.Query) || x.Contents.Contains(request.Query));
            }
            var count = query.Count();
            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/ClaimPrograms/getByID")]
        public ClaimPrograms GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/ClaimPrograms/create")]
        public async Task<ClaimPrograms> CreateBackoffice([FromForm]ClaimPrograms model, [FromForm]IEnumerable<IFormFile> files, [FromForm]IEnumerable<IFormFile> images, [FromForm]IEnumerable<IFormFile> videos)
        {
            if(model != null)
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
                    model.ClaimProgramAttachments.Add(await InsertToAzure(image, model, "Create"));
                    model.FeaturedImageUrl = model.ClaimProgramAttachments.Where(x => x.Title.Contains("IMG")).FirstOrDefault().StorageUrl;
                }
                foreach (var file in files)
                {
                    model.ClaimProgramAttachments.Add(await InsertToAzure(file, model, "Create"));
                }
                foreach (var video in videos)
                {
                    model.ClaimProgramAttachments.Add(await InsertToAzure(video, model, "Create"));
                }

                _appService.Create(model);
            }
            return model;
        }

        [HttpPut("/api/services/app/backoffice/ClaimPrograms/update")]
        public ClaimPrograms EditBackoffice(ClaimPrograms model)
        {
            if (model != null)
            {
                if(model.FeaturedImageUrl == null)
                {
                    model.FeaturedImageUrl = "";
                }
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.Now;

                _appService.Update(model);
            }
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/ClaimPrograms/destroy")]
        public String DestroyBackoffice(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }

        private async Task<ClaimProgramAttachments> InsertToAzure(IFormFile file, ClaimPrograms model, string mode)
        {
            ClaimProgramAttachments attachments = new ClaimProgramAttachments();

            var configuration = new AzureController().GetConnectionToAzure();

            string conn = configuration.GetConnectionString(FLPConsts.AzureConnectionString);

            CloudStorageAccount cloudStorage;
            if (CloudStorageAccount.TryParse(conn, out cloudStorage))
            {
                CloudBlobClient cloudBlobClient = cloudStorage.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("claimprograms");

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
                if (model.ClaimProgramAttachments.Count == 0)
                {
                    namaFile = fileType + "_" + file.FileName + "_" + model.Id + "_" + DateTime.UtcNow.AddHours(7).ToString("yyyyMMdd") + "_1" + path;
                    order = "1";
                }
                else
                {
                    if (mode == "Create")
                    {
                        order = (int.Parse(model.ClaimProgramAttachments.OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
                    }
                    else
                    {
                        order = (int.Parse(_appService.GetById(model.Id).ClaimProgramAttachments.OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
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
                attachments.ClaimProgramId = model.Id;
                attachments.Order = order;
                attachments.Title = namaFile;
                attachments.StorageUrl = cloudBlockBlob.Uri.AbsoluteUri;
                attachments.FileName = file.FileName;
            }

            return attachments;
        }

        #endregion

        #region Claimer
        [HttpGet("/api/services/app/backoffice/ClaimPrograms/getAllClaimers")]
        public BaseResponse GetAllClaimers(Guid guid, [FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);
            var query = _claimerAppService.GetClaimers(guid);
            var count = query.Count();
            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/ClaimPrograms/getClaimerByID")]
        public ClaimProgramClaimers GetClaimerByIDBackoffice(Guid guid)
        {
            return _claimerAppService.GetById(guid);
        }

        [HttpDelete("/api/services/app/backoffice/ClaimPrograms/destroyClaimer")]
        public String DestroyClaimerBackoffice(Guid guid)
        {
            _claimerAppService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }

        [HttpPost("/api/services/app/backoffice/ClaimPrograms/approveClaimer")]
        public String ApproveClaim(Guid id)
        {
            _claimerAppService.Approve(new ApprovalClaimDto { ClaimProgramClaimerId = id, IsApproved = true });

            return "Successfully approved";
        }

        [HttpPost("/api/services/app/backoffice/ClaimPrograms/rejectClaimer")]
        public String RejectClaim(Guid id)
        {
            _claimerAppService.Approve(new ApprovalClaimDto { ClaimProgramClaimerId = id, IsApproved = false });

            return "Successfully rejected";
        }
        #endregion

        #region Attachment
        [HttpGet("/api/services/app/backoffice/ClaimPrograms/getAllAttachments")]
        public List<ClaimProgramAttachments> GetAttachmentBackoffice(Guid modelId, String attachmentType)
        {
            return _appService.GetAllAttachments(modelId).Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.Title.Contains(attachmentType)).OrderBy(x => x.Title).ToList();
        }

        [HttpPut("/api/services/app/backoffice/ClaimPrograms/updateAttachment")]
        public ClaimPrograms UpdateAttachmentBackoffice(Guid Id, IEnumerable<IFormFile> images, IEnumerable<IFormFile> videos, IEnumerable<IFormFile> documents)
        {
            var model = _appService.GetById(Id);
            IEnumerable<IFormFile> files = images.Count() > 0 ? images : videos.Count() > 0 ? videos : documents;

            if (model != null)
            {
                if (files.Count() > 0)
                {
                    foreach (var file in files)
                    {
                        //model.ProgramAttachments.Add(await InsertToAzure(file, model, "Edit"));

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

        [HttpDelete("/api/services/app/backoffice/ClaimPrograms/deleteAttachment")]
        public String DestroyAttachmentBackoffice(Guid modelId)
        {
            _attachmentAppService.SoftDelete(modelId, "admin");
           
            return "Successfully deleted";
        }
        #endregion

        [HttpPost("/api/services/app/backoffice/ClaimPrograms/exportExcel")]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        private ActionResult File(byte[] fileContents, string contentType, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}