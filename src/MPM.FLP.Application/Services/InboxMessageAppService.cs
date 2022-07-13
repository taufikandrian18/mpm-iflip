using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using CorePush.Google;
using Microsoft.EntityFrameworkCore;
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
    public class InboxMessageAppService : FLPAppServiceBase, IInboxMessageAppService
    {
        private readonly IRepository<InboxMessages, Guid> _inboxMessageRepository;
        private readonly IRepository<PushNotificationSubscribers, Guid> _pushNotificationSubscriberRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;

        public InboxMessageAppService(
                                      IRepository<InboxMessages, Guid> inboxMessageRepository,
                                      IRepository<PushNotificationSubscribers, Guid> pushNotificationSubscriberRepository,
                                      IAbpSession abpSession,
                                      LogActivityAppService logActivityAppService)
        {
            _inboxMessageRepository = inboxMessageRepository;
            _pushNotificationSubscriberRepository = pushNotificationSubscriberRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public IQueryable<InboxMessages> GetAll()
        {
            return _inboxMessageRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).Include(y => y.InboxAttachments);
        }


        //public List<Guid> GetAllIds()
        //{
        //    return _inboxMessageRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
        //                    .OrderByDescending(x => x.CreationTime).Select(x => x.Id).ToList();

        //}

        public ICollection<InboxAttachments> GetAllAttachments(Guid id)
        {
            var InboxMessages = _inboxMessageRepository.GetAll().Include(x => x.InboxAttachments);
            var attachments = InboxMessages.FirstOrDefault(x => x.Id == id).InboxAttachments;
            return attachments;
        }

        public InboxMessages GetById(Guid id)
        {
            var InboxMessage = _inboxMessageRepository.GetAll().Include(x => x.InboxAttachments).Include(x => x.InboxRecipients).FirstOrDefault(x => x.Id == id);
            return InboxMessage;
        }

        public void Create(InboxMessages input)
        {
            //_inboxMessageRepository.Insert(input);
            var inboxId = _inboxMessageRepository.InsertAndGetId(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Inbox Message", inboxId, input.Title, LogAction.Create.ToString(), null, input);
            SendInboxNotification(input);
        }

        public void Update(InboxMessages input)
        {
            var oldObject = _inboxMessageRepository.GetAll().AsNoTracking().Include(x => x.InboxAttachments).Include(x => x.InboxRecipients).FirstOrDefault(x => x.Id == input.Id);
            _inboxMessageRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Inbox Message", input.Id, input.Title, LogAction.Update.ToString(), oldObject, input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var InboxMessage = _inboxMessageRepository.FirstOrDefault(x => x.Id == id);
            var oldObject = _inboxMessageRepository.GetAll().AsNoTracking().FirstOrDefault(x => x.Id == id);
            InboxMessage.DeleterUsername = username;
            InboxMessage.DeletionTime = DateTime.Now;
            _inboxMessageRepository.Update(InboxMessage);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "Inbox Message", id, InboxMessage.Title, LogAction.Delete.ToString(), oldObject, InboxMessage);
        }

        async Task SendInboxNotification(InboxMessages inbox)
        {
            foreach (var recipient in inbox.InboxRecipients)
            {
                var subs = _pushNotificationSubscriberRepository.GetAll().Where(x => x.Username == recipient.IDMPM.ToString()).ToList();
                foreach (var s in subs)
                {
                    using (var fcm = new FcmSender(AppConstants.ServerKey, AppConstants.SenderID))
                    {
                        var notification = AppHelpers.CreateNotification(inbox.Contents);
                        await fcm.SendAsync(s.DeviceToken, notification);
                    }
                }
            }
        }
    }
}
