using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;

namespace MPM.FLP.Services.Backoffice
{
    public interface IClaimProgramsController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        ClaimPrograms GetByIDBackoffice(Guid guid);
        Task<ClaimPrograms> CreateBackoffice(ClaimPrograms model, IEnumerable<IFormFile> files, IEnumerable<IFormFile> images, IEnumerable<IFormFile> videos);
        ClaimPrograms EditBackoffice(ClaimPrograms model);
        String DestroyBackoffice(Guid guid);

        BaseResponse GetAllClaimers(Guid guid, Pagination request);
        ClaimProgramClaimers GetClaimerByIDBackoffice(Guid guid);
        String DestroyClaimerBackoffice(Guid guid);
        String ApproveClaim(Guid id);

        List<ClaimProgramAttachments> GetAttachmentBackoffice(Guid modelId, String attachmentType);
        ClaimPrograms UpdateAttachmentBackoffice(Guid Id, IEnumerable<IFormFile> images, IEnumerable<IFormFile> videos, IEnumerable<IFormFile> documents);
        String DestroyAttachmentBackoffice(Guid modelId);
        ActionResult Excel_Export_Save(string contentType, string base64, string fileName);
    }
}