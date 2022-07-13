using Abp.Domain.Repositories;
using Abp.Runtime.Session;
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
    public class ContentBankDetailAppService : FLPAppServiceBase, IContentBankDetailAppService
    {
        private readonly IRepository<ContentBanks, Guid> _repository;
        private readonly IRepository<ContentBankDetails, Guid> _repositoryDetail;
        private readonly IRepository<ContentBankAssignees, Guid> _repositoryAssignee;
        private readonly IAbpSession _abpSession;

        public ContentBankDetailAppService(
            IRepository<ContentBanks, Guid> repository,
            IRepository<ContentBankDetails, Guid> repositoryDetail,
            IRepository<ContentBankAssignees, Guid> repositoryAssignee,
            IAbpSession abpSession)
        {
            _repository = repository;
            _repositoryDetail = repositoryDetail;
            _repositoryAssignee = repositoryAssignee;
            _abpSession = abpSession;
        }

        public List<ContentBankDetails> GetByContentBankId(Guid Id)
        {
            return _repositoryDetail.GetAllList(x => x.GUIDContentBank == Id && x.DeletionTime == null);
        }

        public void Add(ContentBankDetails input)
        {
            input.GUIDContentBank = Guid.NewGuid();
            _repositoryDetail.Insert(input);
        }

        public void Update(ContentBanksDetailsUpdateDto input)
        {
            #region Delete Existing Content Bank Details
            SoftDeleteDetail(input.ContentBankId, input.LastModifierUsername);
            #endregion

            #region Create Content Bank Details
            foreach (var details in input.Details)
            {
                var _details = ObjectMapper.Map<ContentBankDetails>(details);
                _details.GUIDContentBank = input.ContentBankId;
                _details.CreatorUsername = input.LastModifierUsername;
                _details.CreationTime = DateTime.Now;
                _repositoryDetail.Insert(_details);
            }
            #endregion
        }

        private void SoftDeleteDetail(Guid ContentBankId, string LastModifierUsername)
        {
            var details = _repositoryDetail.GetAllList(x => x.GUIDContentBank == ContentBankId && x.DeletionTime == null);
            foreach(var detail in details)
            {
                detail.DeleterUsername = LastModifierUsername;
                detail.DeletionTime = DateTime.Now;
                _repositoryDetail.Update(detail);
            }
        }

        public List<ContentBankDetails> DownloadByContentBankId(List<Guid> Ids)
        {
            var currentUserId = _abpSession.UserId;

            var details = _repositoryDetail.GetAllList(x => Ids.Contains(x.GUIDContentBank) && x.DeletionTime == null);
            foreach(var detail in details)
            {
                var assignee = _repositoryAssignee.GetAll()
                    .Where(x => x.GUIDContentBankDetail == detail.Id
                             && x.GUIDEmployee == currentUserId
                             && x.DeletionTime == null)
                    .FirstOrDefault();

                if (assignee != null)
                {
                    assignee.Status = (int)ContentBankAssigneeStatus.Download;
                    _repositoryAssignee.Update(assignee);
                }
            }

            return details;
        }

        public ContentBankDetails DownloadById(Guid Id)
        {
            var currentUserId = _abpSession.UserId;

            var detail = _repositoryDetail.GetAllList(x => x.Id == Id && x.DeletionTime == null).FirstOrDefault();
            var assignee = _repositoryAssignee.GetAll()
                .Where(x => x.GUIDContentBankDetail == detail.Id
                         && x.GUIDEmployee == currentUserId
                         && x.DeletionTime == null)
                .FirstOrDefault();

            assignee.Status = (int)ContentBankAssigneeStatus.Download;
            _repositoryAssignee.Update(assignee);

            return detail;
        }

        public void ReadContentBankDetail(Guid Id)
        {
            var currentUserId = _abpSession.UserId;

            var assignee = _repositoryAssignee.GetAll()
                .Where(x => x.GUIDContentBankDetail == Id
                         && x.GUIDEmployee == currentUserId
                         && x.DeletionTime == null)
                .FirstOrDefault();

            assignee.Status = (int)ContentBankAssigneeStatus.Read;
            _repositoryAssignee.Update(assignee);
        }

        public ContentBanksDetailsByUserDto GetContentBankDetailByUser(Guid Id)
        {
            var currentUserId = this.AbpSession.UserId;

            var content = _repository.GetAll().FirstOrDefault(x => x.Id == Id);
            var result = new ContentBanksDetailsByUserDto()
            {
                Name = content.Name,
                Caption = content.Caption,
                Description = content.Description,
                Details = new List<ContentBanksDetailAttachment>()
            };

            var details = (from detail in _repositoryDetail.GetAll().Where(x => x.DeletionTime == null & x.GUIDContentBank == Id)
                           join assignee in _repositoryAssignee.GetAll().Where(x => x.DeletionTime == null & x.GUIDEmployee == currentUserId)
                                on detail.Id equals assignee.GUIDContentBankDetail
                           select new ContentBanksDetailAttachment
                           {
                                Id = detail.Id,
                                GUIDContentBank = detail.GUIDContentBank,
                                GUIDContentBankAssignee = assignee.Id,
                                Name = detail.Name,
                                Orders = detail.Orders,
                                Description = detail.Description,
                                Caption = detail.Caption,
                                Extension = detail.Extension,
                                AttachmentURL = detail.AttachmentURL,
                                CreationTime = detail.CreationTime,
                                CreatorUsername = detail.CreatorUsername,
                                LastModificationTime = detail.LastModificationTime,
                                LastModifierUsername = detail.LastModifierUsername,
                                DeletionTime = detail.DeletionTime,
                                DeleterUsername = detail.DeleterUsername
                           }).ToList();

            result.Details.AddRange(details);

            return result;
        }
        
        public void SoftDelete(ContentBanksDetailsDeleteDto input)
        {
            var contentBankDetails = _repositoryDetail.Get(input.Id);

            contentBankDetails.DeleterUsername = input.DeleterUsername;
            contentBankDetails.DeletionTime = DateTime.Now;
            _repositoryDetail.Update(contentBankDetails);
            
        }
    }
}
