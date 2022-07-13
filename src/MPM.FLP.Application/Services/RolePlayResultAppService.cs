using Abp.Authorization;
using Abp.Domain.Repositories;
using CorePush.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class RolePlayResultAppService : FLPAppServiceBase, IRolePlayResultAppService
    {
        private readonly IRepository<RolePlayResults, Guid> _rolePlayResultRepository;
        private readonly IRepository<RolePlays, Guid> _rolePlayRepository;
        private readonly IRepository<InboxMessages, Guid> _inboxMessageRepository;
        private readonly IRepository<InboxRecipients, Guid> _inboxRecipientRepository;
        private readonly IRepository<PushNotificationSubscribers, Guid> _pushNotificationSubscriberRepository;


        public RolePlayResultAppService(IRepository<RolePlayResults, Guid> rolePlayResultRepository,
                                        IRepository<RolePlays, Guid> rolePlayRepository,
                                        IRepository<InboxMessages, Guid> inboxMessageRepository,
                                        IRepository<InboxRecipients, Guid> inboxRecipientRepository,
                                        IRepository<PushNotificationSubscribers, Guid> pushNotificationSubscriberRepository)
        {
            _rolePlayResultRepository = rolePlayResultRepository;
            _rolePlayRepository = rolePlayRepository;
            _inboxMessageRepository = inboxMessageRepository;
            _inboxRecipientRepository = inboxRecipientRepository;
            _pushNotificationSubscriberRepository = pushNotificationSubscriberRepository;
        }

        public ServiceResult Create(RolePlayResultDto input)
        {
            try 
            {
                var id = Guid.NewGuid();
                if (input.Id != null)
                    id = input.Id.Value;
                RolePlayResults results = new RolePlayResults() 
                {
                    Id = id,
                    RolePlayId = input.RolePlayId,
                    IDMPM = input.IDMPM,
                    NamaFLP = input.NamaFLP,
                    KodeDealerMPM = input.KodeDealerMPM,
                    NamaDealerMPM = input.NamaDealerMPM,
                    FLPResult = input.Result,
                    FLPGrade = input.Grade,
                    StorageUrl = input.StorageUrl,
                    YoutubeUrl = input.YoutubeUrl,
                    CreatorUsername = input.NamaFLP,
                    CreationTime = DateTime.UtcNow.AddHours(7),
                    RolePlayResultDetails = new List<RolePlayResultDetails>()
                };

                foreach (var detail in input.RolePlayResultDetailDto) 
                {
                    RolePlayResultDetails resultDetails = new RolePlayResultDetails()
                    {
                        Id = Guid.NewGuid(),
                        RolePlayResultId = id,
                        RolePlayDetailId = detail.RolePlayDetailId,
                        Title = detail.Title,
                        Order = detail.Order,
                        IsMandatoryPlatinum = detail.IsMandatoryPlatinum,
                        IsMandatoryGold = detail.IsMandatoryGold,
                        IsMandatorySilver = detail.IsMandatorySilver,
                        BeforePassed = detail.Passed,
                        BeforeNotPassed = detail.NotPassed,
                        BeforeDismiss = detail.Dismiss,
                        CreatorUsername = input.NamaFLP,
                        CreationTime = DateTime.UtcNow.AddHours(7)
                    };

                    results.RolePlayResultDetails.Add(resultDetails);
                }

                _rolePlayResultRepository.Insert(results);
                
                return new ServiceResult() { IsSuccess = true, Message = "Insert Data Success" };
            }
            catch (Exception e)
            { return new ServiceResult() { IsSuccess = false, Message = e.Message }; }            
        }

        public async Task<UploadResult> Upload(IFormFile file)
        {
            try
            {
                var id = Guid.NewGuid();
                string url = await AppHelpers.InsertAndGetUrlAzure(file, "VID_" + id, "roleplay");
                return new UploadResult { IsSuccess = true, Id = id, Message = url };
            }
            catch (Exception e)
            { return new UploadResult() { IsSuccess = false, Id =  null, Message = e.Message }; }
        }

        public IQueryable<RolePlayResults> GetAll()
        {
            return _rolePlayResultRepository.GetAll().Include(x => x.RolePlayResultDetails);
        }

        public List<RolePlayResults> GetAllItemByUser(int idmpm)
        {
            return _rolePlayResultRepository.GetAll().Include(x => x.RolePlayResultDetails).Where(x => x.IDMPM == idmpm).Include(x => x.RolePlayResultDetails).ToList();
        }

        public RolePlayResults GetById(Guid id)
        {
            return _rolePlayResultRepository.GetAll().Include(x =>x.RolePlayResultDetails).FirstOrDefault(x => x.Id == id);
        }

        public void SoftDelete(Guid id, string username)
        {
            var rolePlayResult = _rolePlayResultRepository.FirstOrDefault(x => x.Id == id);
            rolePlayResult.DeleterUsername = username;
            rolePlayResult.DeletionTime = DateTime.Now;
            _rolePlayResultRepository.Update(rolePlayResult);
        }

        public void Validate(RolePlayResults input)
        {
            _rolePlayResultRepository.Update(input);

            AddResultToInbox(input);
        }

        void AddResultToInbox(RolePlayResults rolePlayResults)
        {
            RolePlays rolePlays = _rolePlayRepository.GetAll().FirstOrDefault(x => x.Id == rolePlayResults.RolePlayId);

            Guid id = Guid.NewGuid();
            string creationTime = rolePlayResults.CreationTime.ToString("dd-MM-yyyy HH:mm");

            InboxMessages inboxMessages = new InboxMessages();
            inboxMessages.Id = id;
            inboxMessages.Title = $"Hasil Validasi Role Play {rolePlays.Title}";
            inboxMessages.Contents = $"Hasil Validasi Role Play {rolePlays.Title} pada {creationTime} : " +
                $"{rolePlayResults.VerificationResult} (Grade : {rolePlayResults.VerificationGrade})";
            inboxMessages.IsRolePlay = true;
            inboxMessages.CreationTime = DateTime.UtcNow.AddHours(7);
            inboxMessages.CreatorUsername = "System";

            _inboxMessageRepository.Insert(inboxMessages);

            InboxRecipients inboxRecipients = new InboxRecipients();
            inboxRecipients.Id = Guid.NewGuid();
            inboxRecipients.InboxMessageId = id;
            inboxRecipients.IDMPM = rolePlayResults.IDMPM;
            inboxRecipients.CreationTime = DateTime.UtcNow.AddHours(7);
            inboxRecipients.CreatorUsername = "System";

            _inboxRecipientRepository.Insert(inboxRecipients);

            SendRolePlayNotification(inboxRecipients.IDMPM.ToString(), inboxMessages.Contents);
        }

        async Task SendRolePlayNotification(string idmpm, string content) 
        {
            var subs = _pushNotificationSubscriberRepository.GetAll().Where(x => x.Username == idmpm).ToList();
            foreach (var s in subs)
            {
                using (var fcm = new FcmSender(AppConstants.ServerKey, AppConstants.SenderID))
                {
                    var notification = AppHelpers.CreateNotification(content);
                    await fcm.SendAsync(s.DeviceToken, notification);
                }
            }
        }
    }
}
