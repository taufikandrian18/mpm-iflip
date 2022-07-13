using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

namespace MPM.FLP.Services.Backoffice
{
    public interface IClubCategoriesController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        ClubCommunityCategories GetByIDBackoffice(Guid guid);
        Task<ClubCommunityCategories> Create(ClubCommunityCategories model, IEnumerable<IFormFile> images);
        Task<ClubCommunityCategories> Edit(ClubCommunityCategories model, IEnumerable<IFormFile> images);
        String DestroyBackoffice(Guid guid);
    }
}