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
    public class BASTAppService : FLPAppServiceBase, IBASTAppService
    {
        private readonly IRepository<BASTs, Guid> _repositoryBAST;
        private readonly IRepository<BASTDetails, Guid> _repositoryBASTDetail;
        private readonly IRepository<BASTCategories, Guid> _repositoryCategory;
        private readonly IRepository<BASTAttachment, Guid> _BASTAttachmentRepository;
        private readonly IRepository<BASTAssignee, Guid> _BASTAssigneeRepository;
        private readonly IRepository<InternalUsers> _internalUserRepository;
        private readonly IRepository<ExternalUsers, Guid> _externalUserRepository;
        private readonly IRepository<PushNotificationSubscribers, Guid> _pushNotificationSubscriberRepository;
        public BASTAppService(
            IRepository<BASTs, Guid> repositoryBAST,
            IRepository<BASTDetails, Guid> repositoryBASTDetail,
            IRepository<BASTCategories, Guid> repositoryCategory,
            IRepository<BASTAttachment, Guid> BASTAttachmentRepository,
            IRepository<BASTAssignee, Guid> BASTAssigneeRepository,
            IRepository<InternalUsers> internalUserRepository,
            IRepository<ExternalUsers, Guid> externalUserRepository,
            IRepository<PushNotificationSubscribers, Guid> pushNotificationSubscriberRepository)
        {
            _repositoryBAST = repositoryBAST;
            _repositoryBASTDetail = repositoryBASTDetail;
            _repositoryCategory = repositoryCategory;
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

        public void Create()
        {
            //var bast = ObjectMapper.Map<BASTs>(input);
            var basts = new BASTs();
            //basts.GUIDCategory = Guid.Parse("540F69FB-7168-4346-9F93-153A454002EC");
            basts.Name = "coba coba";
            //basts.Status = "Dikirim";
            basts.CreationTime = DateTime.Now;
            basts.CreatorUsername = this.AbpSession.UserId.ToString();
            Guid bastId = _repositoryBAST.InsertAndGetId(basts);
            //var bastId = _repositoryBAST.Insert(basts);

            //#region Create bast Details
            //foreach (var details in input.Details)
            //{
            //    var _details = ObjectMapper.Map<BASTDetail>(details);
            //    _details.GUIDBAST = bastId;
            //    _details.CreatorUsername = this.AbpSession.UserId.ToString();
            //    _details.CreationTime = DateTime.Now;
            //    Guid bastDetailId = _repositoryBASTDetail.InsertAndGetId(_details);

            //    #region Create bast Attachment
            //    foreach (var attachment in details.Attachment)
            //    {
            //        var _attachment = ObjectMapper.Map<BASTAttachment>(attachment);
            //        _attachment.GUIDBASTDetail = bastDetailId;
            //        _attachment.CreatorUsername = this.AbpSession.UserId.ToString();
            //        _attachment.CreationTime = DateTime.Now;
            //        _BASTAttachmentRepository.Insert(_attachment);
            //    }
            //    #endregion bast attachment
            //}
            //#endregion bast detail

            //#region Create assignees
            //if (input.IsH1)
            //{
            //    var userInternal = Task.Run(() => _internalUserRepository.GetAll().Where(x => x.Channel == "H1")).Result.ToList();
            //    foreach (var assignee in userInternal)
            //    {
            //        var _assignee = new  BASTAssignee();
            //        _assignee.GUIDBAST = bastId;
            //        _assignee.GUIDEmployee = assignee.AbpUserId;
            //        _assignee.Jabatan = assignee.Jabatan;
            //        _assignee.DealerName = assignee.DealerName;
            //        _assignee.Channel = assignee.Channel;
            //        _assignee.Kota = assignee.DealerKota;
            //        _assignee.CreatorUsername = this.AbpSession.UserId.ToString();
            //        _assignee.CreationTime = DateTime.Now;
            //        _BASTAssigneeRepository.Insert(_assignee);
            //    }
            //}
            //if (input.IsH2)
            //{
            //    var userInternal = Task.Run(() => _internalUserRepository.GetAll().Where(x => x.Channel == "H2")).Result.ToList();
            //    foreach (var assignee in userInternal)
            //    {
            //        var _assignee = new BASTAssignee();
            //        _assignee.GUIDBAST = bastId;
            //        _assignee.GUIDEmployee = assignee.AbpUserId;
            //        _assignee.Jabatan = assignee.Jabatan;
            //        _assignee.DealerName = assignee.DealerName;
            //        _assignee.Channel = assignee.Channel;
            //        _assignee.Kota = assignee.DealerKota;
            //        _assignee.CreatorUsername = this.AbpSession.UserId.ToString();
            //        _assignee.CreationTime = DateTime.Now;
            //        _BASTAssigneeRepository.Insert(_assignee);
            //    }
            //}
            //if (input.IsH3)
            //{
            //    var userExternal = Task.Run(() => _externalUserRepository.GetAll()).Result.ToList();
            //    foreach (var assignee in userExternal)
            //    {
            //        var _assignee = new BASTAssignee();
            //        _assignee.GUIDBAST = bastId;
            //        _assignee.GUIDEmployee = assignee.AbpUserId;
            //        _assignee.Jabatan = assignee.Jabatan;
            //        _assignee.Channel = assignee.Channel;
            //        _assignee.Kota = assignee.Kota;
            //        _assignee.CreatorUsername = this.AbpSession.UserId.ToString();
            //        _assignee.CreationTime = DateTime.Now;
            //        _BASTAssigneeRepository.Insert(_assignee);
            //    }
            //}

            //#endregion
            //var task = Task.Run(() => SendClaimProgramNotification(bast));
        }
        public void Update(BASTUpdateDto input)
        {
            var bast = _repositoryBAST.Get(input.Id);
            bast.Name = input.Name;
            //bast.IsTbsm = input.IsTbsm;
            //bast.IsH1 = input.IsH1;
            //bast.IsH2 = input.IsH2;
            //bast.IsH3 = input.IsH3;
            //bast.KodeAHM = input.KodeAHM;
            //bast.KodeMPM = input.KodeMPM;
            //bast.KodeH3AHM = input.KodeH3AHM;
            //bast.KodeH3MPM = input.KodeH3MPM;
            //bast.RoadLetter = input.RoadLetter;
            //bast.Description = input.Description;
            bast.LastModifierUsername = this.AbpSession.UserId.ToString();
            bast.LastModificationTime = DateTime.Now;

            _repositoryBAST.Update(bast);
        }
        public void SoftDelete(BASTDeleteDto input)
        {
            var bast = _repositoryBAST.Get(input.Id);
            bast.DeleterUsername = this.AbpSession.UserId.ToString();
            bast.DeletionTime = DateTime.Now;
            _repositoryBAST.Update(bast);

            //SoftDeleteDetail(input.Id);
        }

        //private void SoftDeleteDetail(Guid bastId)
        //{
        //    var details = _repositoryBASTDetail.GetAllList(x => x.GUIDBASTs == bastId && x.DeletionTime == null);
        //    foreach (var detail in details)
        //    {
        //        detail.DeleterUsername = this.AbpSession.UserId.ToString();
        //        detail.DeletionTime = DateTime.Now;
        //        _repositoryBASTDetail.Update(detail);
        //    }
        //}

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
