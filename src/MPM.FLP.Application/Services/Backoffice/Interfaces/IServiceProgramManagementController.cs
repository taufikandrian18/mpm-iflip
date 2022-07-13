using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

namespace MPM.FLP.Services.Backoffice
{
    public interface IServiceProgramManagementController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        ServicePrograms GetByIDBackoffice(Guid guid);
        Task<ServicePrograms> Create(ServicePrograms model, IEnumerable<IFormFile> files, IEnumerable<IFormFile> images, IEnumerable<IFormFile> videos);
        Task<ServicePrograms> Edit(ServicePrograms model);
        ServicePrograms UploadAttachment(Guid Id, IEnumerable<IFormFile> images, IEnumerable<IFormFile> videos, IEnumerable<IFormFile> documents);
        String Destroy(Guid guid);
        List<ServiceProgramAttachments> GetAttachmentBackoffice(Guid modelId, String attachmentType);
        String DestroyAttachmentBackoffice(Guid item);
    }
}