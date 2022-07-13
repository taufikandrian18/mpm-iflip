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
    public class ServiceProgramAppService : FLPAppServiceBase, IServiceProgramAppService
    {
        private readonly IRepository<ServicePrograms, Guid> _serviceProgramRepository;
        private readonly IRepository<PushNotificationSubscribers, Guid> _pushNotificationSubscriberRepository;
        private readonly IRepository<InternalUsers> _internalUserRepository;
        private readonly IRepository<ExternalUsers, Guid> _externalUserRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;

        public ServiceProgramAppService(
            IRepository<ServicePrograms, Guid> serviceProgramRepository,
            IRepository<PushNotificationSubscribers, Guid> pushNotificationSubscriberRepository,
            IRepository<InternalUsers> internalUserRepository,
            IRepository<ExternalUsers, Guid> externalUserRepository,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService)
        {
            _serviceProgramRepository = serviceProgramRepository;
            _pushNotificationSubscriberRepository = pushNotificationSubscriberRepository;
            _internalUserRepository = internalUserRepository;
            _externalUserRepository = externalUserRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public IQueryable<ServicePrograms> GetAll()
        {
            return _serviceProgramRepository.GetAll()
                .Where(x => DateTime.Now.Date >= x.StartDate.Date
                    && DateTime.Now.Date <= x.EndDate.Date
                    && string.IsNullOrEmpty(x.DeleterUsername)
                ).Include(y => y.ServiceProgramAttachments);
        }

        public IQueryable<ServicePrograms> GetAllBackoffice()
        {
            return _serviceProgramRepository.GetAll()
                .Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                .Include(y => y.ServiceProgramAttachments);
        }

        public ICollection<ServiceProgramAttachments> GetAllAttachments(Guid id)
        {
            var servicePrograms = _serviceProgramRepository.GetAll().Include(x => x.ServiceProgramAttachments);
            var attachments = servicePrograms.FirstOrDefault(x => x.Id == id).ServiceProgramAttachments;
            return attachments;
        }

        public List<Guid> GetAllIds(string channel)
        {
            var serviceProgram = _serviceProgramRepository.GetAll().Where(x => x.IsPublished
                                                              && DateTime.Now.Date >= x.StartDate.Date
                                                              && DateTime.Now.Date <= x.EndDate.Date
                                                              && string.IsNullOrEmpty(x.DeleterUsername))
                                                    .OrderBy(x => x.EndDate);

            switch (channel)
            {
                case "H1":
                    return serviceProgram.Where(x => x.H1).Select(x => x.Id).ToList();
                case "H2":
                    return serviceProgram.Where(x => x.H2).Select(x => x.Id).ToList();
                case "H3":
                    return serviceProgram.Where(x => x.H3).Select(x => x.Id).ToList();
                default:
                    return serviceProgram.Select(x => x.Id).ToList();
            }

        }

        public ServicePrograms GetById(Guid id)
        {
            var servicePrograms = _serviceProgramRepository.GetAll().Include(x => x.ServiceProgramAttachments).FirstOrDefault(x => x.Id == id);
            return servicePrograms;
        }

        public void Create(ServicePrograms input)
        {
            _serviceProgramRepository.Insert(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Service Program", input.Id, input.Title, LogAction.Create.ToString(), null, input);
            SendServiceProgramNotification(input);
        }

        public void Update(ServicePrograms input)
        {
            var oldObject = _serviceProgramRepository.GetAll().AsNoTracking().Include(x => x.ServiceProgramAttachments).FirstOrDefault(x => x.Id == input.Id);
            _serviceProgramRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Service Program", input.Id, input.Title, LogAction.Update.ToString(), oldObject, input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var oldObject = _serviceProgramRepository.GetAll().AsNoTracking().Include(x => x.ServiceProgramAttachments).FirstOrDefault(x => x.Id == id);
            var serviceProgram = _serviceProgramRepository.FirstOrDefault(x => x.Id == id);
            serviceProgram.DeleterUsername = username;
            serviceProgram.DeletionTime = DateTime.Now;
            _serviceProgramRepository.Update(serviceProgram);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "Service Program", id, serviceProgram.Title, LogAction.Update.ToString(), oldObject, serviceProgram);
        }

        async Task SendServiceProgramNotification(ServicePrograms serviceProgram)
        {
            List<string> deviceTokens = new List<string>();

            if (serviceProgram.H1)
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

            if (serviceProgram.H2)
            {
                deviceTokens.AddRange
                ((
                    from p in _pushNotificationSubscriberRepository.GetAll()
                    join i in _internalUserRepository.GetAll()
                    on p.Username equals i.IDMPM.ToString()
                    where i.Channel == "H3"
                    select p.DeviceToken
                 ).ToList());
            }

            if (serviceProgram.H3)
            {
                deviceTokens.AddRange
                ((
                    from p in _pushNotificationSubscriberRepository.GetAll()
                    join e in _externalUserRepository.GetAll()
                    on p.Username equals e.UserName
                    select p.DeviceToken
                 ).ToList());
            }

            var data = "SERVICEPROGRAM," + serviceProgram.Id + "," + serviceProgram.Title;
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
