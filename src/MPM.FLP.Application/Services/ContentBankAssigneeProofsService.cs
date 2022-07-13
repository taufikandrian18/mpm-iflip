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
    public class ContentBankAssigneeProofsService : FLPAppServiceBase, IContentBankAssigneeProofAppService
    {
        private readonly IRepository<ContentBankAssignees, Guid> _repositoryAssignee;
        private readonly IRepository<ContentBankAssigneeProofs, Guid> _repositoryAssigneeProof;
        private readonly IAbpSession _abpSession;

        public ContentBankAssigneeProofsService(
            IRepository<ContentBankAssignees, Guid> repositoryAssignee,
            IRepository<ContentBankAssigneeProofs, Guid> repositoryAssigneeProof,
            IAbpSession abpSession)
        {
            _repositoryAssignee = repositoryAssignee;
            _repositoryAssigneeProof = repositoryAssigneeProof;
            _abpSession = abpSession;
        }

        public List<ContentBankAssigneeProofs> GetByContentBankDetailId(Guid Id)
        {
            var currentUserId = _abpSession.UserId;
            var assignee = _repositoryAssignee.GetAll().Where(x => x.GUIDContentBankDetail == Id && x.GUIDEmployee == currentUserId && x.DeletionTime == null).FirstOrDefault();

            if (assignee == null)
                return null;
            else
                return _repositoryAssigneeProof.GetAllList(x => x.GUIDContentBankAssignee == assignee.Id && x.DeletionTime == null);
        }

        public void Create(List<ContentBankAssigneeProofsCreateDto> input)
        {
            var currentUserId = _abpSession.UserId;
            foreach (var proof in input)
            {
                var data = ObjectMapper.Map<ContentBankAssigneeProofs>(proof);
                data.CreationTime = DateTime.Now;
                data.GUIDEmployee = currentUserId.HasValue ? currentUserId.Value : 0;
                _repositoryAssigneeProof.Insert(data);

                var assignee = _repositoryAssignee.Get(proof.GUIDContentBankAssignee);
                assignee.Status = (int) ContentBankAssigneeStatus.Upload;
                _repositoryAssignee.Update(assignee);
            }
        }

        public void Update(List<ContentBankAssigneeProofsUpdateDto> input)
        {
            foreach (var proof in input)
            {
                var data = _repositoryAssigneeProof.Get(proof.Id);
                data.Extension = proof.Extension;
                data.AttachmentURL = proof.AttachmentURL;
                data.ViewCount = proof.ViewCount;
                data.UploadDate = proof.UploadDate;
                data.RelatedLink = proof.RelatedLink;
                data.LastModifierUsername = proof.LastModifierUsername;
                data.LastModificationTime = DateTime.Now;

                _repositoryAssigneeProof.Update(data);
            }
        }
    }
}
