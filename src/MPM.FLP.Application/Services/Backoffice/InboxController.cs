using Microsoft.AspNetCore.Mvc;
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

namespace MPM.FLP.Services.Backoffice
{
    public class InboxController : FLPAppServiceBase, IInboxController
    {
        private readonly UserManager _userManager;
        private readonly InboxMessageAppService _appService;
        private readonly InboxAttachmentAppService _attachmentAppService;

        public InboxController(InboxMessageAppService appService, InboxAttachmentAppService attachmentAppService, UserManager userManager, InboxRecipientAppService recipientAppService, DealerAppService dealerAppService, InternalUserAppService internalUserAppService)
        {
            _appService = appService;
            _attachmentAppService = attachmentAppService;
            _userManager = userManager;
        }

        [HttpGet("/api/services/app/backoffice/Inbox/getAll")]
        public BaseResponse GetAllBackoffice([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);
            var query = _appService.GetAll().Where(x => x.CreatorUsername == "admin");

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Title.Contains(request.Query) || x.Contents.Contains(request.Query));
            }
            var count = query.Count();
            var data = query.OrderByDescending(x => x.CreationTime).Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        [HttpGet("/api/services/app/backoffice/Inbox/getByID")]
        public InboxMessages GetByIDBackoffice(Guid guid)
        {
            return _appService.GetById(guid);
        }

        [HttpPost("/api/services/app/backoffice/Inbox/create")]
        public async Task<InboxMessages> Create([FromForm]InboxMessages model, [FromForm]IEnumerable<IFormFile> files, [FromForm]IEnumerable<IFormFile> images,[FromForm] IEnumerable<IFormFile> videos)
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
                        model.InboxAttachments.Add(await InsertToAzure(file, model, "Create"));
                    }

                    model.FeaturedImageUrl = model.InboxAttachments.First().StorageUrl;
                }
                if (files.Count() > 0)
                {
                    foreach (var file in files)
                    {
                        model.InboxAttachments.Add(await InsertToAzure(file, model, "Create"));
                    }
                }
                if (videos.Count() > 0)
                {
                    foreach (var file in videos)
                    {
                        model.InboxAttachments.Add(await InsertToAzure(file, model, "Create"));
                    }
                }

                _appService.Create(model);
            }

            return model;
        }

        [HttpPost("/api/services/app/backoffice/Inbox/recipients")]
        public String InsertRecipient(string id, List<User> selectedUser)
        {
            var model = _appService.GetById(Guid.Parse(id));

            foreach(var user in selectedUser)
            {
                model.InboxRecipients.Add(new InboxRecipients {
                    Id = Guid.NewGuid(),
                    CreationTime = DateTime.UtcNow.AddHours(7),
                    CreatorUsername = "admin",
                    LastModifierUsername = "admin",
                    DeleterUsername = "",
                    InboxMessageId = model.Id,
                    IsRead = false
                });
            }

            _appService.Update(model);
            return "Data berhasil dimasukkan";
        }

        private async Task<InboxAttachments> InsertToAzure(IFormFile file, InboxMessages model, string mode)
        {
            InboxAttachments attachments = new InboxAttachments();

            var configuration = new AzureController().GetConnectionToAzure();

            string conn = configuration.GetConnectionString(FLPConsts.AzureConnectionString);

            CloudStorageAccount cloudStorage;
            if (CloudStorageAccount.TryParse(conn, out cloudStorage))
            {
                CloudBlobClient cloudBlobClient = cloudStorage.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("inbox");

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
                if (model.InboxAttachments.Count == 0)
                {
                    namaFile = fileType + "_"+ file.FileName + "_" + model.Id + "_" + DateTime.UtcNow.AddHours(7).ToString("yyyyMMdd") + "_1" + path;
                    order = "1";
                }
                else
                {
                    if (mode == "Create")
                    {
                        order = (int.Parse(model.InboxAttachments.OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
                    }
                    else
                    {
                        order = (int.Parse(_appService.GetAllAttachments(model.Id).OrderBy(x => x.CreationTime).LastOrDefault().Order) + 1).ToString();
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
                attachments.InboxMessageId = model.Id;
                attachments.Order = order;
                attachments.Title = namaFile;
                attachments.StorageUrl = cloudBlockBlob.Uri.AbsoluteUri;
                attachments.FileName = file.FileName;
            }

            return attachments;
        }

        [HttpPut("/api/services/app/backoffice/Inbox/update")]
        public InboxMessages Edit(InboxMessages model)
        {
            if (model != null)
            {
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.UtcNow.AddHours(7);

                _appService.Update(model);
            }
            return model;
        }

        [HttpPut("/api/services/app/backoffice/Inbox/updateAttachment")]
        public InboxMessages EditAttachment(Guid Id, IEnumerable<IFormFile> files)
        {
            var model = _appService.GetById(Id);

            if (model != null)
            {
                model = _appService.GetById(Id);
                //Check if featuredimgurl empty
                if (string.IsNullOrEmpty(model.FeaturedImageUrl))
                {
                    model.FeaturedImageUrl = _appService.GetAllAttachments(Id).FirstOrDefault(x => string.IsNullOrEmpty(x.DeleterUsername)).StorageUrl;
                }
                model.LastModifierUsername = "admin";
                model.LastModificationTime = DateTime.UtcNow.AddHours(7);
                _appService.Update(model);
            }
            return model;
        }

        [HttpDelete("/api/services/app/backoffice/Inbox/destroy")]
        public String DestroyBackoffice(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }

        [HttpGet("/api/services/app/backoffice/Inbox/getAllAttachment")]
        public List<InboxAttachments> GetAttachmentBackoffice(Guid modelId)
        {
            return _appService.GetAllAttachments(modelId).Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderBy(x => x.Title).ToList();
        }

        [HttpDelete("/api/services/app/backoffice/Inbox/destroyAttachment")]
        public String DestroyAttachment(Guid guid)
        {
            _attachmentAppService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }
    }
}
