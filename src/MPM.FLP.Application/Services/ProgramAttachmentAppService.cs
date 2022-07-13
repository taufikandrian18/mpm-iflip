using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Authorization;
using MPM.FLP.Authorization.Users;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class ProgramAttachmentAppService : FLPAppServiceBase, IProgramAttachmentAppService
    {
        private readonly IRepository<ProgramAttachments, Guid> _programAttachmentRepository;

        public ProgramAttachmentAppService(IRepository<ProgramAttachments, Guid> programAttachmentRepository)
        {
            _programAttachmentRepository = programAttachmentRepository;
        }
       
        public ProgramAttachments GetById(Guid id)
        {
            var programAttachment = _programAttachmentRepository.FirstOrDefault(x => x.Id == id);
            return programAttachment;
        }

        public void Create(ProgramAttachments input)
        {
            _programAttachmentRepository.Insert(input);
        }

        public void Update(ProgramAttachments input)
        {
            _programAttachmentRepository.Update(input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var programAttachment = _programAttachmentRepository.FirstOrDefault(x => x.Id == id);
            programAttachment.DeleterUsername = username;
            programAttachment.DeletionTime = DateTime.Now;
            _programAttachmentRepository.Update(programAttachment);
        }
    }
}
