using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

namespace MPM.FLP.Services.Backoffice
{
    public interface ISalesTalksController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        SalesTalks GetByIDBackoffice(Guid guid);
        Task<SalesTalks> Create(SalesTalks model, IEnumerable<IFormFile> files, IEnumerable<IFormFile> documents);
        Task<SalesTalks> Edit(SalesTalks model);
        SalesTalks EditAttachment(Guid Id, IEnumerable<IFormFile> files, IEnumerable<IFormFile> documents);
        String Destroy(Guid guid);
        List<SalesTalkAttachments> GetAttachments(Guid guid, String attachmentType);
        String DestroyAttachment(Guid item);
    }
}