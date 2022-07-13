using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

namespace MPM.FLP.Services.Backoffice
{
    public interface IArtikelController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        Articles GetByIDBackoffice(Guid guid);
        Task<Articles> CreateBackoffice(Articles model, IEnumerable<IFormFile> files, IEnumerable<IFormFile> images, IEnumerable<IFormFile> videos);
        Articles UpdateBackoffice(Articles model);
        String DestroyBackoffice(Guid guid);
        List<ArticleAttachments> GetAttachmentBackoffice(Guid modelId, String attachmentType);
        Task<Articles> UpdateAttachmentBackoffice(Guid Id, List<string> files, List<string> images, List<string> videos);
        String DestroyAttachmentBackoffice(Guid id);
    }
}
