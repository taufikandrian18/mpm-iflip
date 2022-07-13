using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IClubCommunityCategoryAppService : IApplicationService
    {
        IQueryable<ClubCommunityCategories> GetAll();
        List<ClubCommunityCategories> GetAllClubCommunityCategoryItems();
        ClubCommunityCategories GetById(Guid id);
        void Create(ClubCommunityCategories input);
        void Update(ClubCommunityCategories input);
        void SoftDelete(Guid id, string username);
    }
}
