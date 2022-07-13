using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

namespace MPM.FLP.Services.Backoffice
{
    public interface IProgramManagementController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        SalesPrograms GetByIDBackoffice(Guid guid);
        Task<SalesPrograms> Create(SalesPrograms model, IEnumerable<IFormFile> files, IEnumerable<IFormFile> images, IEnumerable<IFormFile> videos);
        Task<SalesPrograms> Edit(SalesPrograms model);
        SalesPrograms UploadAttachment(Guid Id, IEnumerable<IFormFile> images, IEnumerable<IFormFile> videos, IEnumerable<IFormFile> documents);
        String Destroy(Guid guid);
        List<SalesProgramAttachments> GetAttachmentBackoffice(Guid modelId, String attachmentType);
        String DestroyAttachment(Guid id);
    }
}
