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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]

    public class ClaimProgramAppService : FLPAppServiceBase, IClaimProgramAppService
    {
        private readonly IRepository<ClaimPrograms, Guid> _claimProgramRepository;
        private readonly IRepository<PushNotificationSubscribers, Guid> _pushNotificationSubscriberRepository;
        private readonly IRepository<InternalUsers> _internalUserRepository;
        private readonly IRepository<ExternalUsers, Guid> _externalUserRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;

        public ClaimProgramAppService(
            IRepository<ClaimPrograms, Guid> claimProgramRepository,
            IRepository<PushNotificationSubscribers, Guid> pushNotificationSubscriberRepository,
            IRepository<InternalUsers> internalUserRepository,
            IRepository<ExternalUsers, Guid> externalUserRepository,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService)
        {
            _claimProgramRepository = claimProgramRepository;
            _pushNotificationSubscriberRepository = pushNotificationSubscriberRepository;
            _internalUserRepository = internalUserRepository;
            _externalUserRepository = externalUserRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public IQueryable<ClaimPrograms> GetAll()
        {
            return _claimProgramRepository.GetAll().Where(x=> x.DeletionTime == null).Include(y => y.ClaimProgramAttachments);
        }

        public ICollection<ClaimProgramAttachments> GetAllAttachments(Guid id)
        {
            var ClaimProgram = _claimProgramRepository.GetAll().Include(x => x.ClaimProgramAttachments);
            var attachments = ClaimProgram.FirstOrDefault(x => x.Id == id).ClaimProgramAttachments;
            return attachments;
        }

        public List<Guid> GetAllIds(string channel)
        {
            var claimPrograms =  _claimProgramRepository.GetAll().Where(x => x.IsPublished
                                                         && DateTime.Now.Date >= x.StartDate.Date
                                                         && DateTime.Now.Date <= x.EndDate.Date
                                                         && string.IsNullOrEmpty(x.DeleterUsername))
                                                .OrderByDescending(x => x.EndDate).ToList();
            if (channel == "H2")
                return claimPrograms.Where(x => x.IsH3Ahass.Value == true).Select(x => x.Id).ToList();
            else if (channel == "H3")
                return claimPrograms.Where(x => x.IsH3.Value == true).Select(x => x.Id).ToList();
            else
                return new List<Guid>();
        }

        public ClaimPrograms GetById(Guid id)
        {
            var ClaimProgram = _claimProgramRepository.GetAll().Include(x => x.ClaimProgramAttachments).Include(x => x.ClaimProgramClaimers).FirstOrDefault(x => x.Id == id);
            return ClaimProgram;
        }

        public void Create(ClaimPrograms input)
        {
            _claimProgramRepository.Insert(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Claim Program", input.Id, input.Title, LogAction.Create.ToString(), null, input);
        }

        public void Update(ClaimPrograms input)
        {
            var oldObject = _claimProgramRepository.GetAll().AsNoTracking().Include(x => x.ClaimProgramAttachments).Include(x => x.ClaimProgramClaimers).FirstOrDefault(x => x.Id == input.Id);
            _claimProgramRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Claim Program", input.Id, input.Title, LogAction.Update.ToString(), oldObject, input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var oldObject = _claimProgramRepository.GetAll().AsNoTracking().FirstOrDefault(x => x.Id == id);
            var ClaimProgram = _claimProgramRepository.FirstOrDefault(x => x.Id == id);
            ClaimProgram.DeleterUsername = username;
            ClaimProgram.DeletionTime = DateTime.Now;
            _claimProgramRepository.Update(ClaimProgram);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "Claim Program", id, ClaimProgram.Title, LogAction.Delete.ToString(), oldObject, ClaimProgram);
        }

        async Task SendClaimProgramNotification(ClaimPrograms claimProgram)
        {
            List<string> deviceTokens = new List<string>();

            if (claimProgram.IsH3Ahass.Value)
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

            if (claimProgram.IsH3.Value)
            {
                deviceTokens.AddRange
                ((
                    from p in _pushNotificationSubscriberRepository.GetAll()
                    join e in _externalUserRepository.GetAll()
                    on p.Username equals e.UserName
                    select p.DeviceToken
                 ).ToList());
            }

            var data = "CLAIMPROGRAM," + claimProgram.Id + "," + claimProgram.Title;
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
