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
    public class BrandCampaignAppService : FLPAppServiceBase, IBrandCampaignAppService
    {
        private readonly IRepository<BrandCampaigns, Guid> _brandCampaignRepository;
        private readonly IRepository<PushNotificationSubscribers, Guid> _pushNotificationSubscriberRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;

        public BrandCampaignAppService(
            IRepository<BrandCampaigns, Guid> brandCampaignRepository,
            IRepository<PushNotificationSubscribers, Guid> pushNotificationSubscriberRepository,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService)
        {
            _brandCampaignRepository = brandCampaignRepository;
            _pushNotificationSubscriberRepository = pushNotificationSubscriberRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public IQueryable<BrandCampaigns> GetAll()
        {
            return _brandCampaignRepository.GetAll().Where(x=> x.DeletionTime == null).Include(y => y.BrandCampaignAttachments);
        }

        public ICollection<BrandCampaignAttachments> GetAllAttachments(Guid id)
        {
            var BrandCampaigns = _brandCampaignRepository.GetAll().Include(x => x.BrandCampaignAttachments);
            var attachments = BrandCampaigns.FirstOrDefault(x => x.Id == id).BrandCampaignAttachments;
            return attachments;
        }

        public BrandCampaigns GetLast()
        {
            var BrandCampaign = _brandCampaignRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.IsPublished)
                                                                 .Include(x => x.BrandCampaignAttachments).Last();
            return BrandCampaign;
        }

        public BrandCampaigns GetById(Guid id)
        {
            var BrandCampaign = _brandCampaignRepository.GetAll().Include(x => x.BrandCampaignAttachments).FirstOrDefault(x => x.Id == id);
            return BrandCampaign;
        }

        public void Create(BrandCampaigns input)
        {
            _brandCampaignRepository.Insert(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Brand Campaign", input.Id, input.Title, LogAction.Create.ToString(), null, input);
            SendBrandCampaignNotification(input);
        }

        public void Update(BrandCampaigns input)
        {
            var oldObject = _brandCampaignRepository.GetAll().AsNoTracking().Include(x => x.BrandCampaignAttachments).FirstOrDefault(x => x.Id == input.Id);
            _brandCampaignRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Brand Campaign", input.Id, input.Title, LogAction.Update.ToString(), oldObject, input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var oldObject = _brandCampaignRepository.GetAll().AsNoTracking().Include(x => x.BrandCampaignAttachments).FirstOrDefault(x => x.Id == id);
            var BrandCampaign = _brandCampaignRepository.FirstOrDefault(x => x.Id == id);
            BrandCampaign.DeleterUsername = username;
            BrandCampaign.DeletionTime = DateTime.Now;
            _brandCampaignRepository.Update(BrandCampaign);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "Brand Campaign", id, BrandCampaign.Title, LogAction.Delete.ToString(), oldObject, BrandCampaign);
        }

        async Task SendBrandCampaignNotification(BrandCampaigns campaign)
        {
            List<string> deviceTokens = new List<string>();
            deviceTokens = _pushNotificationSubscriberRepository.GetAll().Select(x => x.DeviceToken).ToList();

            var data = "BRANDCAMPAIGN," + campaign.Id + "," + campaign.Title;
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
