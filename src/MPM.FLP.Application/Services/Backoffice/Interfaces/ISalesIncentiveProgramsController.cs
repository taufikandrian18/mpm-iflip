using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;

namespace MPM.FLP.Services.Backoffice
{
    public interface ISalesIncentiveProgramsController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        SalesIncentivePrograms GetByIDBackoffice(Guid guid);
        Task<SalesIncentivePrograms> Create(SalesIncentivePrograms model, IEnumerable<IFormFile> files, IEnumerable<IFormFile> images, IEnumerable<IFormFile> videos);
        IncentiveUserVM IncentiveUser(Guid id);
        IncentiveUserVM InsertIncentiveUser(IncentiveUserVM model);
        SalesIncentivePrograms Edit(SalesIncentivePrograms model);
        List<SalesIncentiveProgramAttachments> GetAttachments(Guid guid, String attachmentType);
        SalesIncentivePrograms UploadAttachment(Guid Id, IEnumerable<IFormFile> images, IEnumerable<IFormFile> documents, IEnumerable<IFormFile> videos);
        String Destroy(Guid guid);
        String DestroyAttachmentBackoffice(Guid item);
        ActionResult DownloadExcel();
        Task<String> ImportExcel(IEnumerable<IFormFile> files);
    }
}