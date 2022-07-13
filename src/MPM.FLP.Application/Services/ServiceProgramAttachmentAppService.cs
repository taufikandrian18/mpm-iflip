using Abp.Authorization;
using Abp.Domain.Repositories;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class ServiceProgramAttachmentAppService : FLPAppServiceBase, IServiceProgramAttachmentAppService
    {
        private readonly IRepository<ServiceProgramAttachments, Guid> _serviceProgramAttachmentRepository;

        public ServiceProgramAttachmentAppService(IRepository<ServiceProgramAttachments, Guid> serviceProgramAttachmentRepository)
        {
            _serviceProgramAttachmentRepository = serviceProgramAttachmentRepository;
        }

        public ServiceProgramAttachments GetById(Guid id)
        {
            var serviceProgramAttachment = _serviceProgramAttachmentRepository.FirstOrDefault(x => x.Id == id);
            return serviceProgramAttachment;
        }

        public void Create(ServiceProgramAttachments input)
        {
            _serviceProgramAttachmentRepository.Insert(input);
        }

        public void Update(ServiceProgramAttachments input)
        {
            _serviceProgramAttachmentRepository.Update(input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var serviceProgramAttachment = _serviceProgramAttachmentRepository.FirstOrDefault(x => x.Id == id);
            serviceProgramAttachment.DeleterUsername = username;
            serviceProgramAttachment.DeletionTime = DateTime.Now;
            _serviceProgramAttachmentRepository.Update(serviceProgramAttachment);
        }
    }
}
