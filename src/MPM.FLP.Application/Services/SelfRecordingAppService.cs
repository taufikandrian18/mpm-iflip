using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using CorePush.Google;
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
    public class SelfRecordingAppService : FLPAppServiceBase, ISelfRecordingAppService
    {
        private readonly IRepository<SelfRecordings, Guid> _selfRecordingRepository;
        private readonly IRepository<SelfRecordingAssignments, Guid> _selfRecordingAssignmentRepository;
        private readonly IRepository<InternalUsers> _internalUserRepository;
        private readonly IAbpSession _abpSession;
        private readonly IRepository<PushNotificationSubscribers, Guid> _pushNotificationSubscriberRepository;

        public SelfRecordingAppService(IRepository<SelfRecordings, Guid> selfRecordingRepository,
                                  IRepository<SelfRecordingAssignments, Guid> selfRecordingAssignmentRepository,
                                  IAbpSession abpSession,
                                  IRepository<InternalUsers> internalUserRepository,
                                  IRepository<PushNotificationSubscribers, Guid> pushNotificationSubscriberRepository)
        {
            _selfRecordingRepository = selfRecordingRepository;
            _selfRecordingAssignmentRepository = selfRecordingAssignmentRepository;
            _internalUserRepository = internalUserRepository;
            _abpSession = abpSession;
            _pushNotificationSubscriberRepository = pushNotificationSubscriberRepository;
        }

        public void Create(SelfRecordings input)
        {
            _selfRecordingRepository.Insert(input);
        }

        public IQueryable<SelfRecordings> GetAll()
        {
            return _selfRecordingRepository.GetAll().Include(x => x.SelfRecordingDetails).Include(x => x.SelfRecordingResults);
        }

        public List<SelfRecordings> GetAllItems()
        {
            try
            {

                long currentUserId = _abpSession.UserId.Value;
                var internalUser = _internalUserRepository.GetAll().FirstOrDefault(x => x.AbpUserId.Value == currentUserId);
                if (internalUser != null)
                {
                    List<Guid> SelfRecordingIdList = _selfRecordingAssignmentRepository.GetAll()
                                                        .Where(x => x.KodeDealerMPM == internalUser.KodeDealerMPM)
                                                        .Select(x => x.SelfRecordingId).ToList();

                    return _selfRecordingRepository.GetAll().Where(x => SelfRecordingIdList.Contains(x.Id)).OrderBy(x => x.Order)
                                              .Include(x => x.SelfRecordingDetails).ToList();
                }
                return new List<SelfRecordings>();
            }
            catch (Exception)
            { return new List<SelfRecordings>(); }
        }

        public SelfRecordings GetById(Guid id)
        {
            return _selfRecordingRepository.GetAll().FirstOrDefault(x => x.Id == id);
        }

        public void SoftDelete(Guid id, string username)
        {
            var SelfRecording = _selfRecordingRepository.FirstOrDefault(x => x.Id == id);
            SelfRecording.DeleterUsername = username;
            SelfRecording.DeletionTime = DateTime.Now;
            _selfRecordingRepository.Update(SelfRecording);
        }

        public void Update(SelfRecordings input)
        {
            _selfRecordingRepository.Update(input);
        }

        async Task SendSelfRecordingResultNotification(SelfRecordingMessageDto message)
        {
            List<string> deviceTokens = new List<string>();
            deviceTokens = _pushNotificationSubscriberRepository
                .GetAll()
                .Where(x => x.Username == message.Username)
                .Select(x => x.DeviceToken).ToList();

            var data = "SELFRECORDING," + message.Id + "," + message.Title;
            foreach (var deviceToken in deviceTokens)
            {
                using (var fcm = new FcmSender(AppConstants.ServerKey, AppConstants.SenderID))
                {
                    var notification = AppHelpers.CreateNotification(data);
                    await fcm.SendAsync(deviceToken, notification);
                }
            }
        }
    }
}
