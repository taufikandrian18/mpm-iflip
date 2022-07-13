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
    public class RolePlayAppService : FLPAppServiceBase, IRolePlayAppService
    {
        private readonly IRepository<RolePlays, Guid> _rolePlayRepository;
        private readonly IRepository<RolePlayAssignments, Guid> _rolePlayAssignmentRepository;
        private readonly IRepository<InternalUsers> _internalUserRepository;
        private readonly IAbpSession _abpSession;
        private readonly IRepository<PushNotificationSubscribers, Guid> _pushNotificationSubscriberRepository;

        public RolePlayAppService(IRepository<RolePlays, Guid> rolePlayRepository,
                                  IRepository<RolePlayAssignments, Guid> rolePlayAssignmentRepository,
                                  IAbpSession abpSession,
                                  IRepository<InternalUsers> internalUserRepository,
                                  IRepository<PushNotificationSubscribers, Guid> pushNotificationSubscriberRepository)
        {
            _rolePlayRepository = rolePlayRepository;
            _rolePlayAssignmentRepository = rolePlayAssignmentRepository;
            _internalUserRepository = internalUserRepository;
            _abpSession = abpSession;
            _pushNotificationSubscriberRepository = pushNotificationSubscriberRepository;
        }

        public void Create(RolePlays input)
        {
            _rolePlayRepository.Insert(input);
        }

        public IQueryable<RolePlays> GetAll()
        {
            return _rolePlayRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).Include(x => x.RolePlayDetails).Include(x => x.RolePlayResults);
        }

        public List<RolePlays> GetAllItems()
        {
            try
            {

                long currentUserId = _abpSession.UserId.Value;
                var internalUser = _internalUserRepository.GetAll().FirstOrDefault(x => x.AbpUserId.Value == currentUserId);
                if (internalUser != null)
                {
                    List<Guid> roleplayIdList = _rolePlayAssignmentRepository.GetAll()
                                                        .Where(x => x.KodeDealerMPM == internalUser.KodeDealerMPM)
                                                        .Select(x => x.RolePlayId).ToList();
                
                    return _rolePlayRepository.GetAll().Where(x => roleplayIdList.Contains(x.Id)).OrderBy(x => x.Order)
                                              .Include(x => x.RolePlayDetails).ToList();
                }
                return new List<RolePlays>();
            }
            catch (Exception) 
            { return new List<RolePlays>(); }
            
        }

        public RolePlays GetById(Guid id)
        {
            return _rolePlayRepository.GetAll().Include(x=> x.RolePlayResults).FirstOrDefault(x => x.Id == id);
        }

        public void SoftDelete(Guid id, string username)
        {
            var rolePlay = _rolePlayRepository.FirstOrDefault(x => x.Id == id);
            rolePlay.DeleterUsername = username;
            rolePlay.DeletionTime = DateTime.Now;
            _rolePlayRepository.Update(rolePlay);
        }

        public void Update(RolePlays input)
        {
            _rolePlayRepository.Update(input);
        }

        async Task SendRolePlayResultNotification(RolePlayMessageDto message)
        {
            List<string> deviceTokens = new List<string>();
            deviceTokens = _pushNotificationSubscriberRepository
                .GetAll()
                .Where(x => x.Username == message.Username)
                .Select(x => x.DeviceToken).ToList();

            var data = "ROLEPLAY," + message.Id + "," + message.Title;
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
