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
    public class BASTReportAttachmentAppService : FLPAppServiceBase, IBASTReportAttachmentAppService
    {
        private readonly IRepository<BASTReportAttachment, Guid> _BASTReportAttachmentRepository;

        public BASTReportAttachmentAppService(IRepository<BASTReportAttachment, Guid> BASTReportAttachmentRepository)
        {
            _BASTReportAttachmentRepository = BASTReportAttachmentRepository;
        }

        public BaseResponse GetAll([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _BASTReportAttachmentRepository.GetAll().Where(x => x.DeletionTime == null);
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.FileName.Contains(request.Query));
            }

            var count = query.Count();
            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }
        public BASTReportAttachment GetById(Guid id)
        {
            var attachment = _BASTReportAttachmentRepository.FirstOrDefault(x => x.Id == id);
            return attachment;
        }

        public void Create(BASTReportAttachmentCreateDto input)
        {
            var attachment = ObjectMapper.Map<BASTReportAttachment>(input);
            attachment.CreationTime = DateTime.Now;
            attachment.CreatorUsername = this.AbpSession.UserId.ToString();
            _BASTReportAttachmentRepository.Insert(attachment);
        }

        public void Update(BASTReportAttachmentUpdateDto input)
        {
            var attachment = _BASTReportAttachmentRepository.Get(input.Id);
            attachment.GUIDBAST = input.GUIDBAST;
            attachment.GUIDReport = input.GUIDReport;
            attachment.MIME = input.MIME;
            attachment.AttachmentUrl = input.AttachmentUrl;
            attachment.FileName = input.FileName;
            attachment.LastModifierUsername = this.AbpSession.UserId.ToString();
            attachment.LastModificationTime = DateTime.Now;

            _BASTReportAttachmentRepository.Update(attachment);
        }

        public void SoftDelete(Guid id)
        {
            var attachment = _BASTReportAttachmentRepository.FirstOrDefault(x => x.Id == id);
            attachment.DeleterUsername = this.AbpSession.UserId.ToString(); 
            attachment.DeletionTime = DateTime.Now;
            _BASTReportAttachmentRepository.Update(attachment);
        }
    }
}
