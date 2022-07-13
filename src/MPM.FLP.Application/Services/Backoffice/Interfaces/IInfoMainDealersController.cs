using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using System;
using MPM.FLP.Authorization.Users;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MPM.FLP.Services.Backoffice
{
    public interface IInfoMainDealersController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        InfoMainDealers GetByIDBackoffice(Guid guid);
        Task<InfoMainDealers> Create(InfoMainDealers model, IEnumerable<IFormFile> files, IEnumerable<IFormFile> images, IEnumerable<IFormFile> videos);
        InfoMainDealers Edit(InfoMainDealers model);
        InfoMainDealers UploadAttachment(Guid Id, IEnumerable<IFormFile> images, IEnumerable<IFormFile> documents, IEnumerable<IFormFile> videos);
        List<InfoMainDealerAttachments> GetAttachmentBackoffice(Guid modelId);
        String DestroyAttachment(Guid guid);
    }
}