using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

namespace MPM.FLP.Services.Backoffice
{
    public interface IManagementPanduanTeknikalController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        Guides GetByIDBackoffice(Guid guid);
        List<GuideCategories> GetGuideCategories();
        Task<Guides> Create(Guides model, IEnumerable<IFormFile> files, IEnumerable<IFormFile> images, IEnumerable<IFormFile> videos);
        Guides Edit(Guides model);
        String Destroy(Guid guid);
        Guides UploadAttachment(Guid Id, IEnumerable<IFormFile> images, IEnumerable<IFormFile> documents, IEnumerable<IFormFile> videos);
        List<GuideAttachments> GetAttachmentBackoffice(Guid modelId, String attachmentType);
        String DestroyAttachmentBackoffice(Guid item);
    }
}
