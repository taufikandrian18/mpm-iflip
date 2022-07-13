using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;

namespace MPM.FLP.Services.Backoffice
{
    public interface IClubCommunitiesController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        ClubCommunities GetByIDBackoffice(Guid guid);
        ClubCommunities Create(ClubCommunitiesVM data);
        Task<string> Import(IEnumerable<IFormFile> files);
        ClubCommunities EditBackoffice(ClubCommunities model);
        String DestroyBackoffice(Guid guid);
        ActionResult DownloadTemplate();
    }
}