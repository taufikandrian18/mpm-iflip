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
    public class GuideAppService : FLPAppServiceBase, IGuideAppService
    {
        private readonly IRepository<Guides, Guid> _guideRepository;
        private readonly IRepository<PushNotificationSubscribers, Guid> _pushNotificationSubscriberRepository;
        private readonly IRepository<InternalUsers> _internalUserRepository;
        private readonly IRepository<ExternalUsers, Guid> _externalUserRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;

        public GuideAppService(IRepository<Guides, Guid> guideRepository,
                               IRepository<PushNotificationSubscribers, Guid> pushNotificationSubscriberRepository,
                               IRepository<InternalUsers> internalUserRepository,
                               IRepository<ExternalUsers, Guid> externalUserRepository,
                               IAbpSession abpSession,
                               LogActivityAppService logActivityAppService)
        {
            _guideRepository = guideRepository;
            _pushNotificationSubscriberRepository = pushNotificationSubscriberRepository;
            _internalUserRepository = internalUserRepository;
            _externalUserRepository = externalUserRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public IQueryable<Guides> GetAll()
        {
            return _guideRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderBy(x=> x.Order).Include(y => y.GuideAttachments);
        }

        public ICollection<GuideAttachments> GetAllAttachments(Guid id)
        {
            var attachments = new List<GuideAttachments>();
            var guides = _guideRepository.GetAll().OrderBy(x=> x.Order).Include(x => x.GuideAttachments);
            var query  = guides.FirstOrDefault(x => x.Id == id && string.IsNullOrEmpty(x.DeleterUsername));
            if(query != null){
               attachments  = query.GuideAttachments.ToList();
            }
            return attachments;
        }

        public List<Guid> GetAllIds()
        {
            return _guideRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.IsPublished).OrderBy(x => x.Order).ThenBy(x=> x.Id).Select(x => x.Id).ToList();
        }

        public List<Guid> GetAllTechnicalGuideIds(string channel)
        {
            var guides =  _guideRepository.GetAll().Where(x => x.IsTechnicalGuide
                                                            && x.IsPublished
                                                            && string.IsNullOrEmpty(x.DeleterUsername)).OrderBy(x => x.Order);

            switch (channel)
            {
                case "H1":
                    return guides.Where(x => x.H1).Select(x => x.Id).ToList();
                case "H2":
                    return guides.Where(x => x.H2).Select(x => x.Id).ToList();
                case "H3":
                    return guides.Where(x => x.H3).Select(x => x.Id).ToList();
                default:
                    return guides.Select(x => x.Id).ToList();
            }

        }

        public List<Guid> GetAllServiceGuideIds(Guid categoryId, string channel)
        {
            var guides = _guideRepository.GetAll().Where(x => !x.IsTechnicalGuide 
                                                            && x.GuideCategoryId == categoryId 
                                                            && x.IsPublished 
                                                            && string.IsNullOrEmpty(x.DeleterUsername)).OrderBy(x => x.Order);

            switch (channel)
            {
                case "H1":
                    return guides.Where(x => x.H1).Select(x => x.Id).ToList();
                case "H2":
                    return guides.Where(x => x.H2).Select(x => x.Id).ToList();
                case "H3":
                    return guides.Where(x => x.H3).Select(x => x.Id).ToList();
                default:
                    return guides.Select(x => x.Id).ToList();
            }
        }

        public Guides GetById(Guid id)
        {
            var guides = _guideRepository.GetAll().Include(x => x.GuideAttachments).FirstOrDefault(x => x.Id == id);
            return guides;
        }

        public void Create(Guides input, string PageName)
        {
            _guideRepository.Insert(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, PageName, input.Id, input.Title, LogAction.Create.ToString(), null, input);
            SendGuideNotification(input);
        }

        public void Update(Guides input, string PageName)
        {
            var oldObject = _guideRepository.GetAll().AsNoTracking().Include(x => x.GuideAttachments).FirstOrDefault(x => x.Id == input.Id);
            _guideRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, PageName, input.Id, input.Title, LogAction.Update.ToString(), oldObject, input);
        }

        public void SoftDelete(Guid id, string username, string PageName)
        {
            var oldObject = _guideRepository.GetAll().AsNoTracking().Include(x => x.GuideAttachments).FirstOrDefault(x => x.Id == id);
            var guide = _guideRepository.FirstOrDefault(x => x.Id == id);
            guide.DeleterUsername = username;
            guide.DeletionTime = DateTime.Now;
            _guideRepository.Update(guide);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, PageName, id, guide.Title, LogAction.Delete.ToString(), oldObject, guide);
        }

        async Task SendGuideNotification(Guides guide)
        {
            List<string> deviceTokens = new List<string>();

            if (guide.H1)
            {
                deviceTokens.AddRange
                ((
                    from p in _pushNotificationSubscriberRepository.GetAll()
                    join i in _internalUserRepository.GetAll()
                    on p.Username equals i.IDMPM.ToString()
                    where i.Channel == "H1"
                    select p.DeviceToken
                 ).ToList());
            }

            if (guide.H2)
            {
                deviceTokens.AddRange
                ((
                    from p in _pushNotificationSubscriberRepository.GetAll()
                    join i in _internalUserRepository.GetAll()
                    on p.Username equals i.IDMPM.ToString()
                    where i.Channel == "H2"
                    select p.DeviceToken
                 ).ToList());
            }

            if (guide.H3)
            {
                deviceTokens.AddRange
                ((
                    from p in _pushNotificationSubscriberRepository.GetAll()
                    join e in _externalUserRepository.GetAll()
                    on p.Username equals e.UserName
                    select p.DeviceToken
                 ).ToList());
            }

            var data = "PANDUAN," + guide.Id + "," + guide.Title;
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
