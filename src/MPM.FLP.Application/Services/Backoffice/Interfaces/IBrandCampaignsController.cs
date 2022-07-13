using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

namespace MPM.FLP.Services.Backoffice
{
    public interface IBrandCampaignsController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        BrandCampaigns GetByIDBackoffice(Guid guid);
        Task<BrandCampaigns> CreateBackoffice(BrandCampaigns model, IEnumerable<IFormFile> images);
        BrandCampaigns UpdateBackoffice(BrandCampaigns model);
        String DestroyBackoffice(Guid guid);
        List<BrandCampaignAttachments> GetAttachmentBackoffice(Guid modelId);
        BrandCampaigns UpdateAttachmentBackoffice(Guid Id, IEnumerable<IFormFile> files);
        String DestroyAttachmentBackoffice(Guid item);
    }
}