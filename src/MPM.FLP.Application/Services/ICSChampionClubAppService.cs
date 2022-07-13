using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface ICSChampionClubAppService : IApplicationService
    {
        IQueryable<CSChampionClubs> GetAll();
        ICollection<CSChampionClubAttachments> GetAllAttachments(Guid id);
        List<Guid> GetAllIds();
        CSChampionClubs GetById(Guid id);
        void Create(CSChampionClubs input);
        void Update(CSChampionClubs input);
        void SoftDelete(Guid id, string username);
    }
}
