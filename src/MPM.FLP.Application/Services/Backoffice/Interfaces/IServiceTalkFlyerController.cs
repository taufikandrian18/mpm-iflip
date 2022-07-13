using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

namespace MPM.FLP.Services.Backoffice
{
    public interface IServiceTalkFlyerController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        ServiceTalkFlyers GetByIDBackoffice(Guid guid);
        Task<ServiceTalkFlyers> Create(ServiceTalkFlyers model, IEnumerable<IFormFile> files, IEnumerable<IFormFile> documents);
        Task<ServiceTalkFlyers> Edit(ServiceTalkFlyers model);
        ServiceTalkFlyers EditAttachment(Guid Id, IEnumerable<IFormFile> files, IEnumerable<IFormFile> documents);
        String Destroy(Guid guid);
        List<ServiceTalkFlyerAttachments> GetAttachments(Guid guid, String attachmentType);
        String DestroyAttachmentBackoffice(Guid item);
    }
}