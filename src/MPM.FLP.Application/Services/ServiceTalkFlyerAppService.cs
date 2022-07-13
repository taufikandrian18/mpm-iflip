using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using CorePush.Google;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Authorization;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.LogActivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class ServiceTalkFlyerAppService : FLPAppServiceBase, IServiceTalkFlyerAppService
    {
        private readonly IRepository<ServiceTalkFlyers, Guid> _serviceTalkFlyerRepository;
        private readonly IRepository<PushNotificationSubscribers, Guid> _pushNotificationSubscriberRepository;
        private readonly IRepository<InternalUsers> _internalUserRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;

        public ServiceTalkFlyerAppService(
            IRepository<ServiceTalkFlyers, Guid> serviceTalkFlyerRepository,
            IRepository<PushNotificationSubscribers, Guid> pushNotificationSubscriberRepository,
            IRepository<InternalUsers> internalUserRepository,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService)
        {
            _serviceTalkFlyerRepository = serviceTalkFlyerRepository;
            _pushNotificationSubscriberRepository = pushNotificationSubscriberRepository;
            _internalUserRepository = internalUserRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public IQueryable<ServiceTalkFlyers> GetAll()
        {
            return _serviceTalkFlyerRepository.GetAll()
                .Where(x => DateTime.Now.Date >= x.StartDate.Date
                    && DateTime.Now.Date <= x.EndDate.Date
                    && string.IsNullOrEmpty(x.DeleterUsername)
                ).Include(y => y.ServiceTalkFlyerAttachments);
        }

        public IQueryable<ServiceTalkFlyers> GetAllBackoffice()
        {
            return _serviceTalkFlyerRepository.GetAll()
                .Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                .Include(y => y.ServiceTalkFlyerAttachments);
        }

        public ICollection<ServiceTalkFlyerAttachments> GetAllAttachments(Guid id)
        {
            var serviceTalkFlyer = _serviceTalkFlyerRepository.GetAll().Include(x => x.ServiceTalkFlyerAttachments);
            var attachments = serviceTalkFlyer.FirstOrDefault(x => x.Id == id).ServiceTalkFlyerAttachments;
            return attachments;
        }

        public List<Guid> GetAllIds()
        {
            return _serviceTalkFlyerRepository.GetAll().Where(x => x.IsPublished
                                                         && DateTime.Now.Date >= x.StartDate.Date
                                                         && DateTime.Now.Date <= x.EndDate.Date
                                                         && string.IsNullOrEmpty(x.DeleterUsername))
                                                .OrderByDescending(x => x.EndDate).Select(x => x.Id).ToList();
        }

        public ServiceTalkFlyers GetById(Guid id)
        {
            var serviceTalkFlyer = _serviceTalkFlyerRepository.GetAll().Include(x => x.ServiceTalkFlyerAttachments).FirstOrDefault(x => x.Id == id);
            return serviceTalkFlyer;
        }

        public void Create(ServiceTalkFlyers input)
        {
            _serviceTalkFlyerRepository.Insert(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Service Talk", input.Id, input.Title, LogAction.Create.ToString(), null, input);
            SendServiceTalk(input);
        }

        public void Update(ServiceTalkFlyers input)
        {
            var oldObject = _serviceTalkFlyerRepository.GetAll().AsNoTracking().Include(x => x.ServiceTalkFlyerAttachments).FirstOrDefault(x => x.Id == input.Id);
            _serviceTalkFlyerRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Service Talk", input.Id, input.Title, LogAction.Update.ToString(), oldObject, input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var oldObject = _serviceTalkFlyerRepository.GetAll().AsNoTracking().Include(x => x.ServiceTalkFlyerAttachments).FirstOrDefault(x => x.Id == id);
            var serviceTalkFlyer = _serviceTalkFlyerRepository.FirstOrDefault(x => x.Id == id);
            serviceTalkFlyer.DeleterUsername = username;
            serviceTalkFlyer.DeletionTime = DateTime.Now;
            _serviceTalkFlyerRepository.Update(serviceTalkFlyer);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "Service Talk", id, serviceTalkFlyer.Title, LogAction.Delete.ToString(), oldObject, serviceTalkFlyer);
        }

        async Task SendServiceTalk(ServiceTalkFlyers service)
        {
            List<string> deviceTokens = new List<string>();


            deviceTokens.AddRange
            ((
                from p in _pushNotificationSubscriberRepository.GetAll()
                join i in _internalUserRepository.GetAll()
                on p.Username equals i.IDMPM.ToString()
                where i.Channel == "H2"
                select p.DeviceToken
             ).ToList());

            var data = "SERVICETALK," + service.Id + "," + service.Title;
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
