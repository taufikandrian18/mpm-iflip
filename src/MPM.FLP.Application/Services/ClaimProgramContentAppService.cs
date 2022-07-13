using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using CorePush.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Authorization.Users;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.LogActivity;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public class ClaimProgramContentAppService : FLPAppServiceBase, IClaimProgramContentAppService
    {
        private readonly IRepository<ClaimProgramContents, Guid> _repositoryClaimProgramContents;
        private readonly IRepository<ClaimProgramContentAttachments, Guid> _repositoryAttachment;
        private readonly IRepository<PushNotificationSubscribers, Guid> _pushNotificationSubscriberRepository;
        private readonly IRepository<InternalUsers> _internalUserRepository;
        private readonly IRepository<ExternalUsers, Guid> _externalUserRepository;
        private readonly IRepository<ClaimProgramAssignees, Guid> _repositoryAssignees;
        private readonly IRepository<Jabatans, int> _repositoryJabatan;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;
        private readonly IRepository<ClaimProgramContentClaimers, Guid> _claimProgramClaimerRepository;
        private readonly IRepository<User, long> _userRepository;
        public ClaimProgramContentAppService(
            IRepository<ClaimProgramContents, Guid> repositoryClaimProgramContents,
            IRepository<ClaimProgramContentAttachments, Guid> repositoryAttachment,
            IRepository<PushNotificationSubscribers, Guid> pushNotificationSubscriberRepository,
            IRepository<InternalUsers> internalUserRepository,
            IRepository<ExternalUsers, Guid> externalUserRepository,
            IRepository<ClaimProgramAssignees, Guid> repositoryAssignees,
            IRepository<Jabatans, int> repositoryJabatan,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService,
            IRepository<ClaimProgramContentClaimers, Guid> claimProgramClaimerRepository,
            IRepository<User, long> userRepository)
        {
            _repositoryClaimProgramContents = repositoryClaimProgramContents;
            _repositoryAttachment = repositoryAttachment;
            _pushNotificationSubscriberRepository = pushNotificationSubscriberRepository;
            _internalUserRepository = internalUserRepository;
            _repositoryAssignees = repositoryAssignees;
            _repositoryJabatan = repositoryJabatan;
            _externalUserRepository = externalUserRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
            _claimProgramClaimerRepository = claimProgramClaimerRepository;
            _userRepository = userRepository;
        }

        public BaseResponse GetAllAdmin([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _repositoryClaimProgramContents.GetAll().Where(x => x.DeletionTime == null);
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Name.Contains(request.Query));
            }

            var count = query.Count();
            var data = query.Skip(request.Page).Take(request.Limit).OrderByDescending(x => x.CreationTime).ToList();

            return BaseResponse.Ok(data, count);
        }

        public BaseResponse GetUser([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _repositoryClaimProgramContents.GetAll()
                        .Include(x => x.ClaimProgramContentAttachments)
                        .Include(x => x.ClaimProgramAssignees)
                        .Include(x=> x.ClaimProgramContentClaimers)
                        .Where(x => x.DeletionTime == null).Where(x => x.IsPublished);
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Name.Contains(request.Query));
            }

            if (request.IsH1 != null)
            {
                query = query.Where(x => x.IsH1);
            }

            if (request.IsH2 != null)
            {
                query = query.Where(x => x.IsH2);
            }

            if (request.IsH3 != null)
            {
                query = query.Where(x => x.IsH3);
            }

            if (request.IsTBSM != null)
            {
                query = query.Where(x => x.IsTBSM);
            }

            var data = query.Skip(request.Page).Take(request.Limit).FirstOrDefault();

            data.ClaimProgramContentAttachments.OrderBy(x => x.CreationTime);
            data.ClaimProgramAssignees.OrderBy(x => x.CreationTime);

            return BaseResponse.Ok(data, 0);
        }

        public BaseResponse GetAllClaimByUser([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);
            var currentUser = this.AbpSession.UserId;
            var currentDate = DateTime.Now.Date;
            var query = _repositoryClaimProgramContents.GetAll()
                        .Where(x => x.DeletionTime == null)
                        .Where(x => x.IsPublished == true)
                        .Where(x => x.StartDate <= currentDate && currentDate <= x.EndDate);

            //var query = _repositoryClaimProgramContents.GetAll().AsNoTracking()
            //            .Include(x => x.ClaimProgramContentAttachments)
            //            .Include(x => x.ClaimProgramContentClaimers)
            //            .Where(x => x.DeletionTime == null)
            //            .Where(x => x.IsPublished == true)
            //            .Where(x => x.StartDate <= currentDate && currentDate <= x.EndDate);
            //.Where(x => x.ClaimProgramContentClaimers.Where(y => y.EmployeeId == currentUser)
            //                                        .Any());

            //var query = _repositoryClaimProgramContents.GetAll()
            //            .Include(x => x.ClaimProgramContentClaimers)
            //            .Where(x => x.ClaimProgramContentClaimers.Where(y => y.EmployeeId == currentUser)
            //                                                    .Any())
            //            .Select(
            //            x => new ClaimProgramContentsListDto
            //            {
            //                Id = x.Id,
            //                Name = x.Name,
            //                CreationTime = x.CreationTime,
            //                claimer = x.ClaimProgramContentClaimers.ToList()

            //            }).ToList();

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Name.Contains(request.Query));
            }

            if (request.IsH1 != null)
            {
                query = query.Where(x => x.IsH1);
            }

            if (request.IsH2 != null)
            {
                query = query.Where(x => x.IsH2);
            }

            if (request.IsH3 != null)
            {
                query = query.Where(x => x.IsH3);
            }

            if (request.IsTBSM != null)
            {
                query = query.Where(x => x.IsTBSM);
            }

            var count = query.Count();
            
            var data = query.Skip(request.Page).Take(request.Limit).OrderByDescending(x => x.CreationTime).ToList();
            
            var output = new List<ClaimProgramContentsListDto>();

            foreach (var datas in data)
            {
                var claimers = (from claimerss in _claimProgramClaimerRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                                join u in _userRepository.GetAll().Where(x => x.DeletionTime == null)
                                on claimerss.EmployeeId equals u.Id
                                where claimerss.GUIDClaimProgramContent == datas.Id && claimerss.EmployeeId == currentUser
                                select new ClaimProgramContentClaimersDto
                                {
                                    Id = claimerss.Id,
                                    EmployeeId = claimerss.EmployeeId,
                                    Username = u.UserName,
                                    StorageUrl = claimerss.StorageUrl,
                                    OTP = claimerss.OTP,
                                    IsApprove = claimerss.IsApprove,
                                    IsVerified = claimerss.IsVerified,
                                    VerifiedTime = claimerss.VerifiedTime,
                                    EnumStatus = claimerss.EnumStatus
                                }).ToList();

                var attachments = _repositoryAttachment.GetAll()
                        .Where(x => x.DeletionTime == null)
                        .ToList();

                var claim = new ClaimProgramContentsListDto
                {
                    Id = datas.Id,
                    GUIDClaimProgramCampaign = datas.GUIDClaimProgramCampaign,
                    Name = datas.Name,
                    Description = datas.Description,
                    KetentuanPoin = datas.KetentuanPoin,
                    Point = datas.Point,
                    MaximumClaim = datas.Point,
                    IsOtp = datas.IsOtp,
                    IsH1 = datas.IsH1,
                    IsH2 = datas.IsH2,
                    IsH3 = datas.IsH3,
                    IsTBSM = datas.IsTBSM,
                    IsPublished = datas.IsPublished,
                    StartDate = datas.StartDate,
                    EndDate = datas.EndDate,
                    CreationTime = datas.CreationTime,
                    claimer = claimers,
                    attachment = attachments
                };

                output.Add(claim);
            }

            return BaseResponse.Ok(output, count);
        }

        public ClaimProgramContents GetById(Guid Id)
        {
            var data  = _repositoryClaimProgramContents.GetAll()
                .Include(x => x.ClaimProgramContentAttachments)
                .Include(x => x.ClaimProgramAssignees)
                .FirstOrDefault(x => x.Id == Id);
            data.ClaimProgramContentAttachments = data.ClaimProgramContentAttachments.OrderBy(x=> x.CreationTime).ToList();
            data.ClaimProgramAssignees = data.ClaimProgramAssignees.OrderBy(x => x.CreationTime).ToList();
            return data;
        }
        public void Create(ClaimProgramContentsCreateDto input)
        {
            #region Create claim program content
            var claimProgram = ObjectMapper.Map<ClaimProgramContents>(input);
            claimProgram.CreationTime = DateTime.Now;
            claimProgram.CreatorUsername = this.AbpSession.UserId.ToString();

            Guid claimProgramId = _repositoryClaimProgramContents.InsertAndGetId(claimProgram);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Claim Program", claimProgramId, input.Name, LogAction.Create.ToString(), null, claimProgram);
            #endregion

            #region Create attachment
            foreach (var attachment in input.attachment)
            {
                var _attachment = ObjectMapper.Map<ClaimProgramContentAttachments>(attachment);
                _attachment.GUIDClaimProgramContent = claimProgramId;
                _attachment.CreatorUsername = this.AbpSession.UserId.ToString();
                _attachment.CreationTime = DateTime.Now;
                _repositoryAttachment.Insert(_attachment);
            }
            #endregion

            #region Create assignees
             //if (input.IsH1)
             //   {
             //       var userInternal = Task.Run(() => _internalUserRepository.GetAll().Where(x=> x.Channel == "H1")).Result.ToList();
             //       foreach (var assignee in userInternal)
             //       {
             //           var _assignee = new ClaimProgramAssignees();
             //           _assignee.GUIDClaimProgramContent = claimProgramId;
             //           _assignee.IsPassed = false;
             //           _assignee.CreatorUsername = this.AbpSession.UserId.ToString();
             //           _assignee.CreationTime = DateTime.Now;
             //           _repositoryAssignees.Insert(_assignee);
             //       }
             //   }
             //   if (input.IsH2)
             //   {
             //       var userInternal = Task.Run(() => _internalUserRepository.GetAll().Where(x=> x.Channel == "H2")).Result.ToList();
             //       foreach (var assignee in userInternal)
             //       {
             //           var _assignee = new ClaimProgramAssignees();
             //           _assignee.GUIDClaimProgramContent = claimProgramId;
             //           _assignee.IsPassed = false;
             //           _assignee.CreatorUsername = this.AbpSession.UserId.ToString();
             //           _assignee.CreationTime = DateTime.Now;
             //           _repositoryAssignees.Insert(_assignee);
             //       }
             //   }
             //   if (input.IsH3)
             //   {
             //       var userExternal = Task.Run(() => _externalUserRepository.GetAll()).Result.ToList();
             //       foreach (var assignee in userExternal)
             //       {
             //           var _assignee = new ClaimProgramAssignees();
             //           _assignee.GUIDClaimProgramContent = claimProgramId;
             //           _assignee.IsPassed = false;
             //           _assignee.CreatorUsername = this.AbpSession.UserId.ToString();
             //           _assignee.CreationTime = DateTime.Now;
             //           _repositoryAssignees.Insert(_assignee);
             //       }
             //   }
            
            #endregion
            //var task = Task.Run(() => SendClaimProgramNotification(claimProgram));
        }

        public void Update(ClaimProgramContentsUpdateDto input)
        {
            #region Update claim program content
            var claimProgram = _repositoryClaimProgramContents.Get(input.Id);
            var oldObject = _repositoryClaimProgramContents.Get(input.Id);
            claimProgram.Name = input.Name;
            claimProgram.Description = input.Description;
            claimProgram.KetentuanPoin = input.KetentuanPoin;
            claimProgram.Point = input.Point;
            claimProgram.MaximumClaim = input.MaximumClaim;
            claimProgram.IsOtp = input.IsOtp;
            claimProgram.IsH1 = input.IsH1;
            claimProgram.IsH2 = input.IsH2;
            claimProgram.IsH3 = input.IsH3;
            claimProgram.IsTBSM = input.IsTBSM;
            claimProgram.IsPublished = input.IsPublished;
            claimProgram.StartDate = input.StartDate;
            claimProgram.EndDate = input.EndDate;
            claimProgram.LastModifierUsername = this.AbpSession.UserId.ToString();
            claimProgram.LastModificationTime = DateTime.Now;

            _repositoryClaimProgramContents.Update(claimProgram);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Claim Program", input.Id, input.Name, LogAction.Update.ToString(), oldObject, claimProgram);
            #endregion

            #region delete existing attachment
            var attachments = _repositoryAttachment.GetAllList(x => x.GUIDClaimProgramContent == input.Id && x.DeletionTime == null);
            var i = 0;
            foreach (var attachment in attachments)
            {
                _repositoryAttachment.Delete(attachment);
                
                //attachment.Name = input.attachment[i].Name;
                //attachment.Category = input.attachment[i].Category;
                //attachment.AttachmentURL = input.attachment[i].AttachmentURL;
                //attachment.LastModifierUsername = this.AbpSession.UserId.ToString();
                //attachment.LastModificationTime = DateTime.Now;

                //_repositoryAttachment.Update(attachment);
                //i++;
            }
            #endregion

            #region Insert attachment
            foreach (var attachment in input.attachment)
            {
                var _attachment = ObjectMapper.Map<ClaimProgramContentAttachments>(attachment);
                _attachment.GUIDClaimProgramContent = input.Id;
                _attachment.CreatorUsername = this.AbpSession.UserId.ToString();
                _attachment.CreationTime = DateTime.Now;
                _repositoryAttachment.Insert(_attachment);
            }
            #endregion

            #region Update assignees

            //var assignees = _repositoryAssignees.GetAllList(x => x.GUIDClaimProgramContent == claimProgram.Id).FirstOrDefault();
            //    if (assignees != null)
            //        _repositoryAssignees.Delete(assignees);

            //if (input.IsH1)
            //    {
            //        var userInternal = Task.Run(() => _internalUserRepository.GetAll().Where(x=> x.Channel == "H1")).Result.ToList();
            //        foreach (var assignee in userInternal)
            //        {
            //            var _assignee = new ClaimProgramAssignees();
            //            _assignee.GUIDClaimProgramContent = claimProgram.Id;
            //            _assignee.CreatorUsername = this.AbpSession.UserId.ToString();
            //            _assignee.CreationTime = DateTime.Now;
            //            _repositoryAssignees.Insert(_assignee);
            //        }
            //    }
            //    if (input.IsH2)
            //    {
            //        var userInternal = Task.Run(() => _internalUserRepository.GetAll().Where(x=> x.Channel == "H2")).Result.ToList();
            //        foreach (var assignee in userInternal)
            //        {
            //            var _assignee = new ClaimProgramAssignees();
            //            _assignee.GUIDClaimProgramContent = claimProgram.Id;
            //            _assignee.CreatorUsername = this.AbpSession.UserId.ToString();
            //            _assignee.CreationTime = DateTime.Now;
            //            _repositoryAssignees.Insert(_assignee);
            //        }
            //    }
            //    if (input.IsH3)
            //    {
            //        var userExternal = Task.Run(() => _externalUserRepository.GetAll()).Result.ToList();
            //        foreach (var assignee in userExternal)
            //        {
            //            var _assignee = new ClaimProgramAssignees();
            //            _assignee.GUIDClaimProgramContent = claimProgram.Id;
            //            _assignee.CreatorUsername = this.AbpSession.UserId.ToString();
            //            _assignee.CreationTime = DateTime.Now;
            //            _repositoryAssignees.Insert(_assignee);
            //        }
            //    }

            #endregion

        }
        public void UpdateAttachment(ClaimProgramContentAttachmentsUpdateDto input)
        {
            #region Update attachment
            var attachment = _repositoryAttachment.Get(input.Id);
            attachment.Name = input.Name;
            attachment.Category = input.Category;
            attachment.AttachmentURL = input.AttachmentURL;
            attachment.LastModifierUsername = this.AbpSession.UserId.ToString();
            attachment.LastModificationTime = DateTime.Now;

            _repositoryAttachment.Update(attachment);
            #endregion

        }
        public void SoftDelete(ClaimProgramContentsDeleteDto input)
        {
            var claimProgram = _repositoryClaimProgramContents.Get(input.Id);
            var oldObject = _repositoryClaimProgramContents.Get(input.Id);
            claimProgram.DeleterUsername = this.AbpSession.UserId.ToString();
            claimProgram.DeletionTime = DateTime.Now;
            _repositoryClaimProgramContents.Update(claimProgram);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.DeleterUsername, "Claim Program", input.Id, claimProgram.Name, LogAction.Delete.ToString(), oldObject, claimProgram);

            SoftDeleteAttachments(input.Id);
            //SoftDeleteAssignees(input.Id);
        }

        private void SoftDeleteAttachments(Guid ClaimId)
        {
            var attachments = _repositoryAttachment.GetAllList(x => x.GUIDClaimProgramContent == ClaimId && x.DeletionTime == null);
            foreach (var attachment in attachments)
            {
                attachment.DeleterUsername = this.AbpSession.UserId.ToString();
                attachment.DeletionTime = DateTime.Now;
                _repositoryAttachment.Update(attachment);
            }
        }

        private void SoftDeleteAssignees(Guid ClaimId)
        {
            var assignees = _repositoryAssignees.GetAllList(x => x.GUIDClaimProgramContent == ClaimId && x.DeletionTime == null);
            foreach (var assignee in assignees)
            {
                assignee.DeleterUsername = this.AbpSession.UserId.ToString();
                assignee.DeletionTime = DateTime.Now;
                _repositoryAssignees.Update(assignee);
            }
        }
        async Task SendClaimProgramNotification(ClaimProgramContents claimProgram)
        {
            List<string> deviceTokens = new List<string>();

            if (claimProgram.IsH1)
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
            if (claimProgram.IsH2)
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
            if (claimProgram.IsH3)
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

            var data = "CLAIMPROGRAM," + claimProgram.Id + "," + claimProgram.Name;
            foreach (var deviceToken in deviceTokens)
            {
                using (var fcm = new FcmSender(AppConstants.ServerKey, AppConstants.SenderID))
                {
                    var notification = AppHelpers.CreateNotification(data);
                    await fcm.SendAsync(deviceToken, notification);
                }
            }
        }

        public BaseResponse GetAllByCampaign([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _repositoryClaimProgramContents.GetAll().Where(x => x.DeletionTime == null);
            if (!string.IsNullOrEmpty(request.ParentId))
            {
                query = query.Where(x => x.GUIDClaimProgramCampaign == Guid.Parse(request.ParentId));
            }

            var count = query.Count();
            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        //public List<ClaimProgramContentDownloadDto> ExportExcel(DateTime StartDate, DateTime EndDate)
        //{
        //    var query = (from claim in _repositoryClaimProgramContents.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
        //                 join attachment in _repositoryAttachment.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
        //                 on claim.Id equals attachment.GUIDClaimProgramContent
        //                 join assignee in _repositoryAssignees.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
        //                 on claim.Id equals assignee.GUIDClaimProgramContent
        //                 join jabatan in _repositoryJabatan.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
        //                 on assignee.IDJabatan equals jabatan.Id
        //                 select new ClaimProgramContentDownloadDto
        //                 {

        //                 });
        //}
    }
}
