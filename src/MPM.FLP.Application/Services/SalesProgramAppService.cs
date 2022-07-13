using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Castle.Windsor.Installer;
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
    public class SalesProgramAppService : FLPAppServiceBase, ISalesProgramAppService
    {
        private readonly IRepository<SalesPrograms, Guid> _salesProgramRepository;
        private readonly IRepository<PushNotificationSubscribers, Guid> _pushNotificationSubscriberRepository;
        private readonly IRepository<InternalUsers> _internalUserRepository;
        private readonly IRepository<ExternalUsers, Guid> _externalUserRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;

        public SalesProgramAppService(
            IRepository<SalesPrograms, Guid> salesProgramRepository,
            IRepository<PushNotificationSubscribers, Guid> pushNotificationSubscriberRepository,
            IRepository<InternalUsers> internalUserRepository,
            IRepository<ExternalUsers, Guid> externalUserRepository,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService)
        {
            _salesProgramRepository = salesProgramRepository;
            _pushNotificationSubscriberRepository = pushNotificationSubscriberRepository;
            _internalUserRepository = internalUserRepository;
            _externalUserRepository = externalUserRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public IQueryable<SalesPrograms> GetAll()
        {
            return _salesProgramRepository.GetAll()
                .Where(x => x.IsPublished
                    && DateTime.Now.Date >= x.StartDate.Date
                    && DateTime.Now.Date <= x.EndDate.Date
                    && string.IsNullOrEmpty(x.DeleterUsername)
                ).Include(y => y.SalesProgramAttachments);
        }

        public IQueryable<SalesPrograms> GetAllBackoffice()
        {
            return _salesProgramRepository.GetAll()
                .Where(x => string.IsNullOrEmpty(x.DeleterUsername)
                ).Include(y => y.SalesProgramAttachments);
        }

        public ICollection<SalesProgramAttachments> GetAllAttachments(Guid id)
        {
            var salesPrograms = _salesProgramRepository.GetAll().Include(x => x.SalesProgramAttachments);
            var attachments = salesPrograms.FirstOrDefault(x => x.Id == id).SalesProgramAttachments;
            return attachments;
        }

        public List<Guid> GetAllIds(string channel)
        {
            var salesProgram = _salesProgramRepository.GetAll().Where(x => x.IsPublished
                                                              && DateTime.Now.Date >= x.StartDate.Date
                                                              && DateTime.Now.Date <= x.EndDate.Date
                                                              && string.IsNullOrEmpty(x.DeleterUsername))
                                                    .OrderBy(x => x.EndDate);

            switch (channel)
            {
                case "H1":
                    return salesProgram.Where(x => x.H1).Select(x => x.Id).ToList();
                case "H2":
                    return salesProgram.Where(x => x.H2).Select(x => x.Id).ToList();
                case "H3":
                    return salesProgram.Where(x => x.H3).Select(x => x.Id).ToList();
                default:
                    return salesProgram.Select(x => x.Id).ToList();
            }

        }

        public SalesPrograms GetById(Guid id)
        {
            var salesPrograms = _salesProgramRepository.GetAll().Include(x => x.SalesProgramAttachments).FirstOrDefault(x => x.Id == id);
            return salesPrograms;
        }

        public void Create(SalesPrograms input)
        {
            _salesProgramRepository.Insert(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Sales Program", input.Id, input.Title, LogAction.Create.ToString(), null, input);
            SendSalesProgramNotification(input);
        }

        public void Update(SalesPrograms input)
        {
            var oldObject = _salesProgramRepository.GetAll().AsNoTracking().Include(x => x.SalesProgramAttachments).FirstOrDefault(x => x.Id == input.Id);
            _salesProgramRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Sales Program", input.Id, input.Title, LogAction.Update.ToString(), oldObject, input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var oldObject = _salesProgramRepository.GetAll().AsNoTracking().Include(x => x.SalesProgramAttachments).FirstOrDefault(x => x.Id == id);
            var salesProgram = _salesProgramRepository.FirstOrDefault(x => x.Id == id);
            salesProgram.DeleterUsername = username;
            salesProgram.DeletionTime = DateTime.Now;
            _salesProgramRepository.Update(salesProgram);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "Sales Program", id, salesProgram.Title, LogAction.Delete.ToString(), oldObject, salesProgram);
        }

        async Task SendSalesProgramNotification(SalesPrograms salesProgram)
        {
            List<string> deviceTokens = new List<string>();


            if (salesProgram.H1) 
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

            if (salesProgram.H2) 
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

            if (salesProgram.H3) 
            {
                deviceTokens.AddRange
                ((
                    from p in _pushNotificationSubscriberRepository.GetAll()
                    join e in _externalUserRepository.GetAll()
                    on p.Username equals e.UserName
                    select p.DeviceToken
                 ).ToList());
            }

            var data = "SALESPROGRAM," + salesProgram.Id + "," + salesProgram.Title;
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
