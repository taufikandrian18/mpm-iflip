using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IClubCommunityAppService : IApplicationService
    {
        IQueryable<ClubCommunities> GetAll();
        ClubCommunities GetById(Guid id);
        ClubCommunities GetByName(string name);
        void Create(ClubCommunities input);
        void Update(ClubCommunities input);
        void SoftDelete(Guid id, string username);
    }
}
