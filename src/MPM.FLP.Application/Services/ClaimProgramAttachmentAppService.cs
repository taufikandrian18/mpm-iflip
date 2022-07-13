using Abp.Authorization;
using Abp.Domain.Repositories;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class ClaimProgramAttachmentAppService : FLPAppServiceBase, IClaimProgramAttachmentAppService
    {
        private readonly IRepository<ClaimProgramAttachments, Guid> _claimProgramAttachmentRepository;

        public ClaimProgramAttachmentAppService(IRepository<ClaimProgramAttachments, Guid> claimProgramAttachmentRepository)
        {
            _claimProgramAttachmentRepository = claimProgramAttachmentRepository;
        }

        public ClaimProgramAttachments GetById(Guid id)
        {
            var ClaimProgramAttachment = _claimProgramAttachmentRepository.FirstOrDefault(x => x.Id == id);
            return ClaimProgramAttachment;
        }

        public void Create(ClaimProgramAttachments input)
        {
            _claimProgramAttachmentRepository.Insert(input);
        }

        public void Update(ClaimProgramAttachments input)
        {
            _claimProgramAttachmentRepository.Update(input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var ClaimProgramAttachment = _claimProgramAttachmentRepository.FirstOrDefault(x => x.Id == id);
            ClaimProgramAttachment.DeleterUsername = username;
            ClaimProgramAttachment.DeletionTime = DateTime.Now;
            _claimProgramAttachmentRepository.Update(ClaimProgramAttachment);
        }
    }
}
