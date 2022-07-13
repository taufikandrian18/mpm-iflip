using Abp.Authorization;
using Abp.Domain.Repositories;
using MPM.FLP.Authorization;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class ServiceTalkFlyerAttachmentAppService : FLPAppServiceBase, IServiceTalkFlyerAttachmentAppService
    {
        private readonly IRepository<ServiceTalkFlyerAttachments, Guid> _serviceTalkFlyerAttachmentRepository;

        public ServiceTalkFlyerAttachmentAppService(IRepository<ServiceTalkFlyerAttachments, Guid> serviceTalkFlyerAttachmentRepository)
        {
            _serviceTalkFlyerAttachmentRepository = serviceTalkFlyerAttachmentRepository;
        }

        public IQueryable<ServiceTalkFlyerAttachments> GetAll(Guid id)
        {
            return _serviceTalkFlyerAttachmentRepository.GetAll().Where(x => x.ServiceTalkFlyerId == id && string.IsNullOrEmpty(x.DeleterUsername));
        }

        public ServiceTalkFlyerAttachments GetById(Guid id)
        {
            var serviceTalkFlyerAttachment = _serviceTalkFlyerAttachmentRepository.FirstOrDefault(x => x.Id == id);
            return serviceTalkFlyerAttachment;
        }

        public void Create(ServiceTalkFlyerAttachments input)
        {
            _serviceTalkFlyerAttachmentRepository.Insert(input);
        }

        public void Update(ServiceTalkFlyerAttachments input)
        {
            _serviceTalkFlyerAttachmentRepository.Update(input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var serviceTalkFlyerAttachment = _serviceTalkFlyerAttachmentRepository.FirstOrDefault(x => x.Id == id);
            serviceTalkFlyerAttachment.DeleterUsername = username;
            serviceTalkFlyerAttachment.DeletionTime = DateTime.Now;
            _serviceTalkFlyerAttachmentRepository.Update(serviceTalkFlyerAttachment);
        }
    }
}
