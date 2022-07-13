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
    public class CSChampionClubAppService : FLPAppServiceBase, ICSChampionClubAppService
    {
        private readonly IRepository<CSChampionClubs, Guid> _csChampionClubRepository;
        private readonly IRepository<PushNotificationSubscribers, Guid> _pushNotificationSubscriberRepository;
        private readonly IRepository<InternalUsers> _internalUserRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;

        public CSChampionClubAppService(
            IRepository<CSChampionClubs, Guid> csChampionClubRepository,
            IRepository<PushNotificationSubscribers, Guid> pushNotificationSubscriberRepository,
            IRepository<InternalUsers> internalUserRepository,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService)
        {
            _csChampionClubRepository = csChampionClubRepository;
            _pushNotificationSubscriberRepository = pushNotificationSubscriberRepository;
            _internalUserRepository = internalUserRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public IQueryable<CSChampionClubs> GetAll()
        {
            return _csChampionClubRepository.GetAll().Where(x=> x.DeletionTime == null)
                    .Include(y => y.CSChampionClubAttachments);
        }

        public ICollection<CSChampionClubAttachments> GetAllAttachments(Guid id)
        {
            var csChampionClubs = _csChampionClubRepository.GetAll().Include(x => x.CSChampionClubAttachments);
            var attachments = csChampionClubs.FirstOrDefault(x => x.Id == id).CSChampionClubAttachments;
            return attachments;
        }


        public List<Guid> GetAllIds()
        {
            return _csChampionClubRepository.GetAll().Where(x => x.IsPublished && string.IsNullOrEmpty(x.DeleterUsername))
                            .OrderByDescending(x => x.CreationTime).Select(x => x.Id).ToList();

        }

        public CSChampionClubs GetById(Guid id)
        {
            var csChampionClub = _csChampionClubRepository.GetAll()
                                    .Include(x => x.CSChampionClubAttachments)
                                    .FirstOrDefault(x => x.Id == id);

            return csChampionClub;
        }

        public void Create(CSChampionClubs input)
        {
            _csChampionClubRepository.Insert(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "CS Champion Club", input.Id, input.Title, LogAction.Create.ToString(), null, input);
            SendCSChampionClubNotification(input);
        }

        public void Update(CSChampionClubs input)
        {
            var oldObject = _csChampionClubRepository.GetAll().AsNoTracking().Include(x => x.CSChampionClubAttachments).FirstOrDefault(x => x.Id == input.Id);
            _csChampionClubRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "CS Champion Club", input.Id, input.Title, LogAction.Update.ToString(), oldObject, input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var oldObject = _csChampionClubRepository.GetAll().AsNoTracking().FirstOrDefault(x => x.Id == id);
            var csChampionClub = _csChampionClubRepository.FirstOrDefault(x => x.Id == id);
            csChampionClub.DeleterUsername = username;
            csChampionClub.DeletionTime = DateTime.Now;
            _csChampionClubRepository.Update(csChampionClub);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "CS Champion Club", id, csChampionClub.Title, LogAction.Delete.ToString(), oldObject, csChampionClub);
        }

        async Task SendCSChampionClubNotification(CSChampionClubs csChampionClub)
        {
            List<string> deviceTokens = new List<string>();

            deviceTokens.AddRange
                ((
                    from p in _pushNotificationSubscriberRepository.GetAll()
                    join i in _internalUserRepository.GetAll()
                    on p.Username equals i.IDMPM.ToString()
                    select p.DeviceToken
                 ).ToList());


            var data = "CSCHAMPIONSCLUB," + csChampionClub.Id + "," + csChampionClub.Title;
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
