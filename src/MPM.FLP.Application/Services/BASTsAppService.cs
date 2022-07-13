using Abp.Domain.Repositories;
using CorePush.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public class BASTsAppService : FLPAppServiceBase, IBASTsAppService
    {
        private readonly IRepository<BASTs, Guid> _repositoryBAST;
        private readonly IRepository<BASTDetails, Guid> _repositoryBASTDetail;
        private readonly IRepository<BASTAttachment, Guid> _BASTAttachmentRepository;
        private readonly IRepository<BASTAssignee, Guid> _BASTAssigneeRepository;
        private readonly IRepository<InternalUsers> _internalUserRepository;
        private readonly IRepository<ExternalUsers, Guid> _externalUserRepository;
        private readonly IRepository<PushNotificationSubscribers, Guid> _pushNotificationSubscriberRepository;
        public BASTsAppService(
            IRepository<BASTs, Guid> repositoryBAST,
            IRepository<BASTDetails, Guid> repositoryBASTDetail,
            IRepository<BASTAttachment, Guid> BASTAttachmentRepository,
            IRepository<BASTAssignee, Guid> BASTAssigneeRepository,
            IRepository<InternalUsers> internalUserRepository,
            IRepository<ExternalUsers, Guid> externalUserRepository,
            IRepository<PushNotificationSubscribers, Guid> pushNotificationSubscriberRepository)
        
        {
            _repositoryBAST = repositoryBAST;
            _repositoryBASTDetail = repositoryBASTDetail;
            _BASTAttachmentRepository = BASTAttachmentRepository;
            _BASTAssigneeRepository = BASTAssigneeRepository;
            _internalUserRepository = internalUserRepository;
            _externalUserRepository = externalUserRepository;
            _pushNotificationSubscriberRepository = pushNotificationSubscriberRepository;
        }

        public BaseResponse GetAll([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _repositoryBAST.GetAll().Where(x => x.DeletionTime == null);
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Name.Contains(request.Query));
            }

            var count = query.Count();
            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        public BASTs GetById(Guid Id)
        {
            var data = _repositoryBAST.GetAll()
                        .FirstOrDefault(x => x.Id == Id);
            return data;
        }

        public void Create(BASTCreateDto input)
        {
            var bast = ObjectMapper.Map<BASTs>(input);
            bast.Status = "Dikirim";
            bast.CreationTime = DateTime.Now;
            bast.CreatorUsername = this.AbpSession.UserId.ToString();
            Guid bastId = _repositoryBAST.InsertAndGetId(bast);

            #region Create bast Details
            foreach (var details in input.details)
            {
                var _details = ObjectMapper.Map<BASTDetails>(details);
                _details.BASTsId = bastId;
                _details.CreatorUsername = this.AbpSession.UserId.ToString();
                _details.CreationTime = DateTime.Now;
                Guid bastDetailId = _repositoryBASTDetail.InsertAndGetId(_details);

                #region Create bast Attachment
                foreach (var attachment in details.Attachment)
                {
                    var _attachment = ObjectMapper.Map<BASTAttachment>(attachment);
                    _attachment.BASTDetailsId = bastDetailId;
                    _attachment.CreatorUsername = this.AbpSession.UserId.ToString();
                    _attachment.CreationTime = DateTime.Now;
                    _BASTAttachmentRepository.Insert(_attachment);
                }
                #endregion
            }
            #endregion

            #region Create assignees
            if (input.IsH1)
            {
                var userInternal = Task.Run(() => _internalUserRepository.GetAll().Where(x => x.Channel == "H1")).Result.ToList();
                foreach (var assignee in userInternal)
                {
                    var _assignee = new BASTAssignee();
                    _assignee.BASTsId = bastId;
                    _assignee.GUIDEmployee = assignee.AbpUserId;
                    _assignee.Jabatan = assignee.Jabatan;
                    _assignee.DealerName = assignee.DealerName;
                    _assignee.Channel = assignee.Channel;
                    _assignee.Kota = assignee.DealerKota;
                    _assignee.CreatorUsername = this.AbpSession.UserId.ToString();
                    _assignee.CreationTime = DateTime.Now;
                    _BASTAssigneeRepository.Insert(_assignee);
                }
            }
            if (input.IsH2)
            {
                var userInternal = Task.Run(() => _internalUserRepository.GetAll().Where(x => x.Channel == "H2")).Result.ToList();
                foreach (var assignee in userInternal)
                {
                    var _assignee = new BASTAssignee();
                    _assignee.BASTsId = bastId;
                    _assignee.GUIDEmployee = assignee.AbpUserId;
                    _assignee.Jabatan = assignee.Jabatan;
                    _assignee.DealerName = assignee.DealerName;
                    _assignee.Channel = assignee.Channel;
                    _assignee.Kota = assignee.DealerKota;
                    _assignee.CreatorUsername = this.AbpSession.UserId.ToString();
                    _assignee.CreationTime = DateTime.Now;
                    _BASTAssigneeRepository.Insert(_assignee);
                }
            }
            if (input.IsH3)
            {
                var userExternal = Task.Run(() => _externalUserRepository.GetAll()).Result.ToList();
                foreach (var assignee in userExternal)
                {
                    var _assignee = new BASTAssignee();
                    _assignee.BASTsId = bastId;
                    _assignee.GUIDEmployee = assignee.AbpUserId;
                    _assignee.Jabatan = assignee.Jabatan;
                    _assignee.Channel = assignee.Channel;
                    _assignee.Kota = assignee.Kota;
                    _assignee.CreatorUsername = this.AbpSession.UserId.ToString();
                    _assignee.CreationTime = DateTime.Now;
                    _BASTAssigneeRepository.Insert(_assignee);
                }
            }

            #endregion
            //var task = Task.Run(() => SendClaimProgramNotification(bast));
        }

        async Task SendClaimProgramNotification(BASTs bast)
        {
            List<string> deviceTokens = new List<string>();

            //if (bast.IsH1)
            //{
            //    deviceTokens.AddRange
            //    ((
            //        from p in _pushNotificationSubscriberRepository.GetAll()
            //        join i in _internalUserRepository.GetAll()
            //        on p.Username equals i.IDMPM.ToString()
            //        where i.Channel == "H1"
            //        select p.DeviceToken
            //     ).ToList());
            //}
            //if (bast.IsH2)
            //{
            //    deviceTokens.AddRange
            //    ((
            //        from p in _pushNotificationSubscriberRepository.GetAll()
            //        join i in _internalUserRepository.GetAll()
            //        on p.Username equals i.IDMPM.ToString()
            //        where i.Channel == "H2"
            //        select p.DeviceToken
            //     ).ToList());
            //}
            //if (bast.IsH3)
            //{
            //    deviceTokens.AddRange
            //    ((
            //        from p in _pushNotificationSubscriberRepository.GetAll()
            //        join i in _internalUserRepository.GetAll()
            //        on p.Username equals i.IDMPM.ToString()
            //        where i.Channel == "H3"
            //        select p.DeviceToken
            //     ).ToList());
            //}

            var data = "BAST," + bast.Id + "," + bast.Name;
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
