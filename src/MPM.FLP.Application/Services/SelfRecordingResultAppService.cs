using Abp.Authorization;
using Abp.Domain.Repositories;
using CorePush.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using Org.BouncyCastle.Asn1.BC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class SelfRecordingResultAppService : FLPAppServiceBase, ISelfRecordingResultAppService
    {
        private readonly IRepository<SelfRecordingResults, Guid> _selfRecordingResultRepository;
        private readonly IRepository<SelfRecordings, Guid> _selfRecordingRepository;
        private readonly IRepository<InboxMessages, Guid> _inboxMessageRepository;
        private readonly IRepository<InboxRecipients, Guid> _inboxRecipientRepository;
        private readonly IRepository<PushNotificationSubscribers, Guid> _pushNotificationSubscriberRepository;

        public SelfRecordingResultAppService(IRepository<SelfRecordingResults, Guid> selfRecordingResultRepository,
                                             IRepository<SelfRecordings, Guid> selfRecordingRepository,
                                             IRepository<InboxMessages, Guid> inboxMessageRepository,
                                             IRepository<InboxRecipients, Guid> inboxRecipientRepository,
                                             IRepository<PushNotificationSubscribers, Guid> pushNotificationSubscriberRepository)
        {
            _selfRecordingResultRepository = selfRecordingResultRepository;
            _selfRecordingRepository = selfRecordingRepository;
            _inboxMessageRepository = inboxMessageRepository;
            _inboxRecipientRepository = inboxRecipientRepository;
            _pushNotificationSubscriberRepository = pushNotificationSubscriberRepository;
        }

        public ServiceResult Create(SelfRecordingResultDto input)
        {
            try
            {
                var id = Guid.NewGuid();
                if (input.Id != null) 
                    id = input.Id.Value;
                SelfRecordingResults results = new SelfRecordingResults()
                {
                    Id = id,
                    SelfRecordingId = input.SelfRecordingId,
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
                    SelfRecordingResultDetails = new List<SelfRecordingResultDetails>()
                };

                foreach (var detail in input.SelfRecordingResultDetailDto)
                {
                    SelfRecordingResultDetails resultDetails = new SelfRecordingResultDetails()
                    {
                        Id = Guid.NewGuid(),
                        SelfRecordingResultId = id,
                        SelfRecordingDetailId = detail.SelfRecordingDetailId,
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

                    results.SelfRecordingResultDetails.Add(resultDetails);
                }

                _selfRecordingResultRepository.Insert(results);

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
                string url = await AppHelpers.InsertAndGetUrlAzure(file, "VID_" + id, "selfrecording");
                return new UploadResult { IsSuccess = true, Id = id, Message = url };
            }
            catch (Exception e)
            { return new UploadResult() { IsSuccess = false, Id = null, Message = e.Message }; }
        }

        public IQueryable<SelfRecordingResults> GetAll()
        {
            return _selfRecordingResultRepository.GetAll().Include(x => x.SelfRecordingResultDetails);
        }

        public List<SelfRecordingResults> GetAllItemByUser(int idmpm)
        {
            return _selfRecordingResultRepository.GetAll().Include(x => x.SelfRecordingResultDetails).Where(x => x.IDMPM == idmpm).Include(x => x.SelfRecordingResultDetails).ToList();
        }

        public SelfRecordingResults GetById(Guid id)
        {
            return _selfRecordingResultRepository.GetAll().Include(x => x.SelfRecordingResultDetails).FirstOrDefault(x => x.Id == id);
        }

        public void SoftDelete(Guid id, string username)
        {
            var SelfRecordingResult = _selfRecordingResultRepository.FirstOrDefault(x => x.Id == id);
            SelfRecordingResult.DeleterUsername = username;
            SelfRecordingResult.DeletionTime = DateTime.Now;
            _selfRecordingResultRepository.Update(SelfRecordingResult);
        }

        public void Validate(SelfRecordingResults input)
        {
            _selfRecordingResultRepository.Update(input);
            
            AddResultToInbox(input);

            
        }

        void AddResultToInbox(SelfRecordingResults selfRecordingResults)
        {
            SelfRecordings selfRecordings = _selfRecordingRepository.GetAll().FirstOrDefault(x => x.Id == selfRecordingResults.SelfRecordingId);

            Guid id = Guid.NewGuid();
            string creationTime = selfRecordingResults.CreationTime.ToString("dd-MM-yyyy HH:mm");

            InboxMessages inboxMessages = new InboxMessages();
            inboxMessages.Id = id;
            inboxMessages.Title = $"Hasil Validasi Self Recording {selfRecordings.Title}";
            inboxMessages.Contents = $"Hasil Validasi Self Recording {selfRecordings.Title} pada {creationTime} : " +
                $"{selfRecordingResults.VerificationResult} (Grade : {selfRecordingResults.VerificationGrade})";
            inboxMessages.IsSelfRecording = true;
            inboxMessages.CreationTime = DateTime.UtcNow.AddHours(7);
            inboxMessages.CreatorUsername = "System";

            _inboxMessageRepository.Insert(inboxMessages);

            InboxRecipients inboxRecipients = new InboxRecipients();
            inboxRecipients.Id = Guid.NewGuid();
            inboxRecipients.InboxMessageId = id;
            inboxRecipients.IDMPM = selfRecordingResults.IDMPM;
            inboxRecipients.CreationTime = DateTime.UtcNow.AddHours(7);
            inboxRecipients.CreatorUsername = "System";

            _inboxRecipientRepository.Insert(inboxRecipients);

            SendSelfRecordingNotification(inboxRecipients.IDMPM.ToString(), inboxMessages.Contents);
        }

        async Task SendSelfRecordingNotification(string idmpm, string content)
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
