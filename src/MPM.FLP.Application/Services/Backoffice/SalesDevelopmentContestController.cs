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
    public class SalesDevelopmentContestController : FLPAppServiceBase, ISalesDevelopmentContestController
    {
        private readonly UserManager _userManager;
        private readonly InboxMessageAppService _appService;
        private readonly InboxAttachmentAppService _attachmentAppService;
        private readonly InboxRecipientAppService _recipientAppService;
        private readonly DealerAppService _dealerAppService;
        private readonly InternalUserAppService _internalUserAppService;

        public SalesDevelopmentContestController(InboxMessageAppService appService, InboxAttachmentAppService attachmentAppService, UserManager userManager, InboxRecipientAppService recipientAppService, DealerAppService dealerAppService, InternalUserAppService internalUserAppService)
        {
            _appService = appService;
            _attachmentAppService = attachmentAppService;
            _recipientAppService = recipientAppService;
            _userManager = userManager;
            _dealerAppService = dealerAppService;
            _internalUserAppService = internalUserAppService;
        }

        [HttpGet("/api/services/app/backoffice/SalesDevelopmentContest/get_channels")]
        public List<string> Cascading_Get_Channel()
        {
            return _dealerAppService.GetChannel().ToList();
        }

        [HttpGet("/api/services/app/backoffice/SalesDevelopmentContest/get_karesidenan")]
        public List<string> Cascading_Get_Karesidenan(string channel)
        {
            if (string.IsNullOrEmpty(channel))
            {
                return _dealerAppService.GetKaresidenanH1().ToList();
            }

            return _dealerAppService.GetKaresidenanHC3(channel).ToList();
        }

        [HttpGet("/api/services/app/backoffice/SalesDevelopmentContest/get_kota")]
        public List<string> Cascading_Get_Kota(string channel, string karesidenan)
        {
            if (string.IsNullOrEmpty(karesidenan) && string.IsNullOrEmpty(channel))
            {
                return _dealerAppService.GetKotaH2().ToList();
            }
            else if(string.IsNullOrEmpty(channel))
            {
                return _dealerAppService.GetKotaH1(karesidenan).ToList();
            }
            
            return _dealerAppService.GetKotaHC3(channel, karesidenan).ToList();
        }

        [HttpGet("/api/services/app/backoffice/SalesDevelopmentContest/get_dealer")]
        public List<Dealers> Cascading_Get_Dealer(string channel, string karesidenan, string kota)
        {
            return _dealerAppService.GetAll().Where(x => x.Channel == channel && x.Kota == kota).ToList();
        }

        [HttpGet("/api/services/app/backoffice/SalesDevelopmentContest/get_user")]
        public List<InternalUsers> Get_User(string channel, string karesidenan, string kota, string dealer)
        {
            List<InternalUsers> internalUser = new List<InternalUsers>();

            var user = _userManager.Users.FirstOrDefault(x => x.UserName == "admin");
            var roles = _userManager.GetRolesAsync(user).Result.ToList();

            string resource = "";
            if (roles.FirstOrDefault().Contains("H1"))
            {
                resource = "H1";
                if (string.IsNullOrEmpty(kota) && string.IsNullOrEmpty(dealer))
                {
                    internalUser = _internalUserAppService.GetInternalUserByKaresidenanH1(karesidenan).ToList();
                }
                else if (string.IsNullOrEmpty(dealer))
                {
                    internalUser = _internalUserAppService.GetInternalUserByKota(resource, kota).ToList();
                }
                else
                {
                    internalUser = _internalUserAppService.GetAll().Where(x => x.Channel == resource && x.DealerKota == kota && x.DealerName == dealer).ToList();
                }
            }
            else if (roles.FirstOrDefault().Contains("H2"))
            {
                resource = "H2";
                
                if (string.IsNullOrEmpty(dealer))
                {
                    internalUser = _internalUserAppService.GetInternalUserByKota(resource, kota).ToList();
                }
                else
                {
                    internalUser = _internalUserAppService.GetAll().Where(x => x.Channel == resource && x.DealerKota == kota && x.DealerName == dealer).ToList();
                }
            }
            else if (roles.FirstOrDefault().Contains("HC3"))
            {
                resource = "HC3";
                if (string.IsNullOrEmpty(karesidenan) && string.IsNullOrEmpty(kota) && string.IsNullOrEmpty(dealer))
                {
                    internalUser = _internalUserAppService.GetInternalUserByChannel(channel).ToList();
                }
                else if (string.IsNullOrEmpty(kota) && string.IsNullOrEmpty(dealer))
                {
                    internalUser = _internalUserAppService.GetInternalUserByKaresidenanHC3(channel,karesidenan).ToList();
                }
                else if (string.IsNullOrEmpty(dealer))
                {
                    internalUser = _internalUserAppService.GetInternalUserByKota(channel, kota).ToList();
                }
                else
                {
                    internalUser = _internalUserAppService.GetAll().Where(x => x.Channel == channel && x.DealerKota == kota && x.DealerName == dealer).ToList();
                }
            }
            
            return internalUser;
        }

        [HttpPost("/api/services/app/backoffice/SalesDevelopmentContest/create")]
        public async Task<InboxMessages> Create([FromForm]InboxMessages model, [FromForm]IEnumerable<IFormFile> files, [FromForm]IEnumerable<IFormFile> images, [FromForm]IEnumerable<IFormFile> videos)
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

        [HttpPost("/api/services/app/backoffice/SalesDevelopmentContest/insertRecipients")]
        public String InsertRecipient(string id, List<InternalUsersVM> selectedUser)
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
                    IDMPM = user.Idmpm,
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

        [HttpGet("/api/services/app/backoffice/SalesDevelopmentContest/get_users")]
        public List<InternalUsersVM> GetUsers(InboxMessages model)
        {
            var tmp = _appService.GetById(model.Id);
            List<InternalUsersVM> internalUsers = new List<InternalUsersVM>();
            foreach(var recipient in tmp.InboxRecipients)
            {
                var user = _internalUserAppService.GetAll().SingleOrDefault(x => x.IDMPM == recipient.IDMPM);
                internalUsers.Add(new InternalUsersVM()
                {
                    Idmpm = user.IDMPM,
                    Nama = user.Nama
                });
            }

            return internalUsers;
        }

        [HttpGet("/api/services/app/backoffice/SalesDevelopmentContest/getAttachments")]
        public List<InboxAttachments> Grid_ReadAttachment(InboxMessages model)
        {
            return _appService.GetById(model.Id).InboxAttachments.ToList();
        }

        [HttpPut("/api/services/app/backoffice/SalesDevelopmentContest/update")]
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

        [HttpPut("/api/services/app/backoffice/SalesDevelopmentContest/updateAttachment")]
        public InboxMessages EditAttachment(Guid Id, IEnumerable<IFormFile> files)
        {
            var model = _appService.GetById(Id);

            if (model != null)
            {
                if (files.Count() > 0)
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
            }
            
            return model;
        }

        [HttpGet("/api/services/app/backoffice/SalesDevelopmentContest/getAll")]
        public List<InboxMessages> Grid_Read()
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

            return _appService.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.CreatorUsername.ToLower() == "admin").OrderByDescending(x => x.CreationTime).ToList();
        }

        [HttpDelete("/api/services/app/backoffice/SalesDevelopmentContest/destroy")]
        public String Grid_Destroy(Guid guid)
        {
            _appService.SoftDelete(guid, "admin");
            return "Successfully deleted";
        }

        [HttpGet("/api/services/app/backoffice/SalesDevelopmentContest/getAttachmentById")]
        public List<InboxAttachments> GridAttachment_Read(Guid modelId)
        {
            return _appService.GetAllAttachments(modelId).Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderBy(x => x.Title).ToList();
        }

        [HttpDelete("/api/services/app/backoffice/SalesDevelopmentContest/destroyAttachment")]
        public String GridAttachment_Destroy(Guid modelId)
        {
            _attachmentAppService.SoftDelete(modelId, "admin");
            return "Successfully deleted";
        }
    }
}