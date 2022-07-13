using Abp.Domain.Repositories;
using Abp.Runtime.Session;
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
    public class ContentBankAppService : FLPAppServiceBase, IContentBankAppService
    {
        private readonly IRepository<ContentBanks, Guid> _repositoryContent;
        private readonly IRepository<ContentBankDetails, Guid> _repositoryDetail;
        private readonly IRepository<ContentBankAssignees, Guid> _repositoryAssignee;
        private readonly InternalUserAppService _internalUserAppService;
        private readonly ExternalUserAppService _externalUserAppService;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;

        public ContentBankAppService(
            IRepository<ContentBanks, Guid> repositoryContent,
            IRepository<ContentBankDetails, Guid> repositoryDetail,
            IRepository<ContentBankAssignees, Guid> repositoryAssignee,
            InternalUserAppService internalUserAppService,
            ExternalUserAppService externalUserAppService,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService)
        {
            _repositoryContent = repositoryContent;
            _repositoryDetail = repositoryDetail;
            _repositoryAssignee = repositoryAssignee;
            _internalUserAppService = internalUserAppService;
            _externalUserAppService = externalUserAppService;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public BaseResponse GetAll([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _repositoryContent.GetAll().Where(x => x.DeletionTime == null);
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Name.Contains(request.Query));
            }

            if(request.IsH1.HasValue){
                query = query.Where(x=> x.H1);
            }

            if(request.IsH2.HasValue){
                query = query.Where(x=> x.H2);
            }

            if(request.IsH3.HasValue){
                query = query.Where(x=> x.H3);
            }

            var count = query.Count();
            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        public ContentBanks GetById(Guid Id)
        {
            return _repositoryContent.GetAll()
                .Include(x => x.ContentBankDetails).ThenInclude(x => x.ContentBankAssignees)
                .Where(x=> x.ContentBankDetails.Any(z=> z.DeletionTime == null))
                .FirstOrDefault(x => x.Id == Id);
        }

        public List<ContentBankResponse> GetByCategory(Guid ContentBankCategoryId, bool? isH1, bool? isH2, bool? isH3)
        {
            var data = (from content in _repositoryContent.GetAll().Where(x => x.DeletionTime == null & x.GUIDContentBankCategory == ContentBankCategoryId)
                        join tmp in _repositoryDetail.GetAll().Where(x => x.DeletionTime == null)
                        on content.Id equals tmp.GUIDContentBank into _detail
                        from detail in _detail.Where(x=> x.Extension != "mp4").OrderBy(x => x.Orders).Take(1)
                        where (isH1 == null || content.H1 == isH1)
                            && (isH2 == null || content.H2 == isH2)
                            && (isH3 == null || content.H3 == isH3)
                        select new ContentBankResponse
                        {
                            Id = content.Id,
                            AttachmentURL = detail.AttachmentURL,
                            Name = content.Name,
                            Description = content.Description
                        }).ToList();
            return data;
        }

        public void Create(ContentBanksCreateDto input)
        {
            #region Create Content Bank
            var contentBank = ObjectMapper.Map<ContentBanks>(input);
            contentBank.CreationTime = DateTime.Now;
            
            Guid contentBankId = _repositoryContent.InsertAndGetId(contentBank);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "ContentBank", contentBankId, input.Name, LogAction.Create.ToString(), null, contentBank);
            #endregion

            #region Create Content Bank Details
            foreach(var details in input.Details)
            {
                var _details = ObjectMapper.Map<ContentBankDetails>(details);
                _details.GUIDContentBank = contentBankId;
                _details.CreatorUsername = input.CreatorUsername;
                _details.CreationTime = DateTime.Now;
                Guid contentBankDetailId = _repositoryDetail.InsertAndGetId(_details);

                //TODO: Bagian ini akan dipindah ke halaman assignment
                #region Create Content Bank Assignees
                if (input.H1)
                {
                    var userInternal = Task.Run(() => _internalUserAppService.GetAllInternalUsers("H1")).Result.ToList();
                    foreach(var user in userInternal)
                    {
                    var _assignee = new ContentBankAssignees()
                    {
                        GUIDContentBankDetail = contentBankDetailId,
                            GUIDEmployee = user.AbpUserId.HasValue ? user.AbpUserId.Value : 0,
                            KodeDealerMPM = user.KodeDealerMPM,
                            Status = (int)ContentBankAssigneeStatus.Assign,
                            CreatorUsername = input.CreatorUsername,
                            CreationTime = DateTime.Now
                        };
                        _repositoryAssignee.Insert(_assignee);
                    }
                }
                if (input.H2)
                {
                    var userInternal = Task.Run(() => _internalUserAppService.GetAllInternalUsers("H2")).Result.ToList();
                    foreach (var user in userInternal)
                    {
                        var _assignee = new ContentBankAssignees()
                        {
                            GUIDContentBankDetail = contentBankDetailId,
                            GUIDEmployee = user.AbpUserId.HasValue ? user.AbpUserId.Value : 0,
                            KodeDealerMPM = user.KodeDealerMPM,
                            Status = (int)ContentBankAssigneeStatus.Assign,
                            CreatorUsername = input.CreatorUsername,
                            CreationTime = DateTime.Now
                        };
                        _repositoryAssignee.Insert(_assignee);
                    }
                }
                if (input.H3)
                {
                    var userExternal = Task.Run(() => _externalUserAppService.GetAll()).Result.ToList();
                    foreach (var user in userExternal)
                    {
                        var _assignee = new ContentBankAssignees()
                        {
                            GUIDContentBankDetail = contentBankDetailId,
                            GUIDEmployee = user.AbpUserId,
                            Status = (int)ContentBankAssigneeStatus.Assign,
                            CreatorUsername = input.CreatorUsername,
                            CreationTime = DateTime.Now
                        };
                        _repositoryAssignee.Insert(_assignee);
                    }
                }
                #endregion
            }
            #endregion
        }

        public void Update(ContentBanksUpdateDto input)
        {
            #region Update Content Bank
            var contentBank = _repositoryContent.Get(input.Id);
            var oldObject = _repositoryContent.Get(input.Id);

            contentBank.Name = input.Name;
            contentBank.GUIDContentBankCategory = input.GUIDContentBankCategory;
            contentBank.Description = input.Description;
            contentBank.Caption = input.Caption;
            contentBank.ReadingTime = input.ReadingTime;
            contentBank.H1 = input.H1;
            contentBank.H2 = input.H2;
            contentBank.H3 = input.H3;
            contentBank.StartDate = input.StartDate;
            contentBank.EndDate = input.EndDate;
            contentBank.IsPublished = input.IsPublished;
            contentBank.LastModifierUsername = input.LastModifierUsername;
            contentBank.LastModificationTime = DateTime.Now;

            _repositoryContent.Update(contentBank);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "ContentBank", input.Id, input.Name, LogAction.Update.ToString(), oldObject, contentBank);
            #endregion

            #region Create Content Bank Assignees
            var detail = _repositoryDetail.GetAllList(x => x.GUIDContentBank == input.Id);
            foreach(var _detail in detail)
            {
                var assignee = _repositoryAssignee.GetAllList(x => x.GUIDContentBankDetail == _detail.Id).FirstOrDefault();
                if (assignee != null)
                    _repositoryAssignee.Delete(assignee);

                if (input.H1)
                {
                    var userInternal = Task.Run(() => _internalUserAppService.GetAllInternalUsers("H1")).Result.ToList();
                    foreach (var user in userInternal)
                    {
                        var _assignee = new ContentBankAssignees()
                        {
                            GUIDContentBankDetail = _detail.Id,
                            GUIDEmployee = user.AbpUserId.HasValue ? user.AbpUserId.Value : 0,
                            KodeDealerMPM = user.KodeDealerMPM,
                            Status = (int)ContentBankAssigneeStatus.Assign,
                            CreatorUsername = input.LastModifierUsername,
                            CreationTime = DateTime.Now
                        };
                        _repositoryAssignee.Insert(_assignee);
                    }
                }
                if (input.H2)
                {
                    var userInternal = Task.Run(() => _internalUserAppService.GetAllInternalUsers("H2")).Result.ToList();
                    foreach (var user in userInternal)
                    {
                        var _assignee = new ContentBankAssignees()
                        {
                            GUIDContentBankDetail = _detail.Id,
                            GUIDEmployee = user.AbpUserId.HasValue ? user.AbpUserId.Value : 0,
                            KodeDealerMPM = user.KodeDealerMPM,
                            Status = (int)ContentBankAssigneeStatus.Assign,
                            CreatorUsername = input.LastModifierUsername,
                            CreationTime = DateTime.Now
                        };
                        _repositoryAssignee.Insert(_assignee);
                    }
                }
                if (input.H3)
                {
                    var userExternal = Task.Run(() => _externalUserAppService.GetAll()).Result.ToList();
                    foreach (var user in userExternal)
                    {
                        var _assignee = new ContentBankAssignees()
                        {
                            GUIDContentBankDetail = _detail.Id,
                            GUIDEmployee = user.AbpUserId,
                            Status = (int)ContentBankAssigneeStatus.Assign,
                            CreatorUsername = input.LastModifierUsername,
                            CreationTime = DateTime.Now
                        };
                        _repositoryAssignee.Insert(_assignee);
                    }
                }
            }
            #endregion
        }

        public void SoftDelete(ContentBanksDeleteDto input)
        {
            var contentBank = _repositoryContent.Get(input.Id);
            var oldObject = _repositoryContent.Get(input.Id);

            contentBank.DeleterUsername = input.DeleterUsername;
            contentBank.DeletionTime = DateTime.Now;
            _repositoryContent.Update(contentBank);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.DeleterUsername, "ContentBank", input.Id, contentBank.Name, LogAction.Delete.ToString(), oldObject, contentBank);

            SoftDeleteDetail(input.Id, input.DeleterUsername);
        }

        private void SoftDeleteDetail(Guid ContentBankId, string LastModifierUsername)
        {
            var details = _repositoryDetail.GetAllList(x => x.GUIDContentBank == ContentBankId && x.DeletionTime == null);
            foreach(var detail in details)
            {
                detail.DeleterUsername = LastModifierUsername;
                detail.DeletionTime = DateTime.Now;
                _repositoryDetail.Update(detail);

                var assignees = _repositoryAssignee.GetAllList(x => x.GUIDContentBankDetail == detail.Id); 
                foreach(var assignee in assignees)
                {
                    assignee.DeleterUsername = LastModifierUsername;
                    assignee.DeletionTime = DateTime.Now;
                    _repositoryAssignee.Update(assignee);
                }
            }
        }
    }
}
