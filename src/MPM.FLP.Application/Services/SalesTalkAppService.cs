using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using CorePush.Google;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Authorization;
using MPM.FLP.Authorization.Users;
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
    public class SalesTalkAppService : FLPAppServiceBase, ISalesTalkAppService
    {
        private readonly IRepository<SalesTalks, Guid> _salesTalkRepository;
        private readonly IRepository<PushNotificationSubscribers, Guid> _pushNotificationSubscriberRepository;
        private readonly IRepository<InternalUsers> _internalUserRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;

        public SalesTalkAppService(
            IRepository<SalesTalks, Guid> salesTalkRepository,
            IRepository<PushNotificationSubscribers, Guid> pushNotificationSubscriberRepository,
            IRepository<InternalUsers> internalUserRepository,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService)
        {
            _salesTalkRepository = salesTalkRepository;
            _pushNotificationSubscriberRepository = pushNotificationSubscriberRepository;
            _internalUserRepository = internalUserRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public IQueryable<SalesTalks> GetAll()
        {
            return _salesTalkRepository.GetAll()
                .Where(x => DateTime.Now.Date >= x.StartDate.Date
                    && DateTime.Now.Date <= x.EndDate.Date
                    && string.IsNullOrEmpty(x.DeleterUsername)
                ).Include(y => y.SalesTalkAttachments);
        }

        public ICollection<SalesTalkAttachments> GetAllAttachments(Guid id)
        {
            var salesTalk = _salesTalkRepository.GetAll().Include(x => x.SalesTalkAttachments);
            var attachments = salesTalk.FirstOrDefault(x => x.Id == id).SalesTalkAttachments;
            return attachments;
        }

        public List<Guid> GetAllIds()
        {
            return _salesTalkRepository.GetAll().Where(x => x.IsPublished
                                                         && DateTime.Now.Date >= x.StartDate.Date
                                                         && DateTime.Now.Date <= x.EndDate.Date
                                                         && string.IsNullOrEmpty(x.DeleterUsername))
                                                .OrderByDescending(x => x.EndDate).Select(x => x.Id).ToList();
        }

        public SalesTalks GetById(Guid id)
        {
            var salesTalk = _salesTalkRepository.GetAll().Include(x => x.SalesTalkAttachments).FirstOrDefault(x => x.Id == id);
            return salesTalk;
        }

        public void Create(SalesTalks input)
        {
            _salesTalkRepository.Insert(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Sales Talk", input.Id, input.Title, LogAction.Create.ToString(), null, input);
            SendSalesTalk(input);
        }

        public void Update(SalesTalks input)
        {
            var oldObject = _salesTalkRepository.GetAll().AsNoTracking().Include(x => x.SalesTalkAttachments).FirstOrDefault(x => x.Id == input.Id);
            _salesTalkRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Sales Talk", input.Id, input.Title, LogAction.Update.ToString(), oldObject, input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var oldObject = _salesTalkRepository.GetAll().AsNoTracking().Include(x => x.SalesTalkAttachments).FirstOrDefault(x => x.Id == id);
            var salesTalk = _salesTalkRepository.FirstOrDefault(x => x.Id == id);
            salesTalk.DeleterUsername = username;
            salesTalk.DeletionTime = DateTime.Now;
            _salesTalkRepository.Update(salesTalk);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "Sales Talk", id, salesTalk.Title, LogAction.Delete.ToString(), oldObject, salesTalk);
        }

        async Task SendSalesTalk(SalesTalks sales)
        {
            List<string> deviceTokens = new List<string>();


            deviceTokens.AddRange
            ((
                from p in _pushNotificationSubscriberRepository.GetAll()
                join i in _internalUserRepository.GetAll()
                on p.Username equals i.IDMPM.ToString()
                where i.Channel == "H1"
                select p.DeviceToken
             ).ToList());

            var data = "SALESTALK," + sales.Id + "," + sales.Title;
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
