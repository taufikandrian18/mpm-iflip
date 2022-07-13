using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

namespace MPM.FLP.Services.Backoffice
{
    public interface IManagementPanduanLayananController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        Guides GetByIDBackoffice(Guid guid);
        List<GuideCategories> GetGuideCategories();
        Task<Guides> Create(GuidesVM data, IEnumerable<IFormFile> files, IEnumerable<IFormFile> images, IEnumerable<IFormFile> videos);
        Guides Edit(GuidesVM data);
        String Destroy(Guid guid);
        Task<Guides> UploadAttachmentAsync(Guid Id, List<string> images, List<string> documents, List<string> videos);
        List<GuideAttachments> GetAttachmentBackoffice(Guid modelId, String attachmentType);
        String DestroyAttachmentBackoffice(Guid item);
    }
}
