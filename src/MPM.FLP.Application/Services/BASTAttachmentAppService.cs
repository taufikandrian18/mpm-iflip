using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class BASTAttachmentAppService : FLPAppServiceBase, IBASTAttachmentAppService
    {
        private readonly IRepository<BASTAttachment, Guid> _BASTAttachmentRepository;

        public BASTAttachmentAppService(IRepository<BASTAttachment, Guid> BASTAttachmentRepository)
        {
            _BASTAttachmentRepository = BASTAttachmentRepository;
        }

        public BaseResponse GetAll([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _BASTAttachmentRepository.GetAll().Where(x => x.DeletionTime == null);
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.FileName.Contains(request.Query));
            }

            var count = query.Count();
            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }
        public BASTAttachment GetById(Guid id)
        {
            var attachment = _BASTAttachmentRepository.FirstOrDefault(x => x.Id == id);
            return attachment;
        }

        public void Create(BASTAttachmentCreateDto input)
        {
            var attachment = ObjectMapper.Map<BASTAttachment>(input);
            attachment.CreationTime = DateTime.Now;
            attachment.CreatorUsername = this.AbpSession.UserId.ToString();
            _BASTAttachmentRepository.Insert(attachment);
        }

        public void Update(BASTAttachmentUpdateDto input)
        {
            var attachment = _BASTAttachmentRepository.Get(input.Id);
            attachment.BASTDetailsId = input.GUIDBASTDetail;
            attachment.AttachmentUrl = input.AttachmentUrl;
            attachment.FileName = input.FileName;
            attachment.LastModifierUsername = this.AbpSession.UserId.ToString();
            attachment.LastModificationTime = DateTime.Now;

            _BASTAttachmentRepository.Update(attachment);
        }

        public void SoftDelete(Guid id)
        {
            var attachment = _BASTAttachmentRepository.FirstOrDefault(x => x.Id == id);
            attachment.DeleterUsername = this.AbpSession.UserId.ToString(); 
            attachment.DeletionTime = DateTime.Now;
            _BASTAttachmentRepository.Update(attachment);
        }
    }
}
