using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IAchievementAppService : IApplicationService
    {
        IQueryable<Achievements> GetAll();
        IQueryable<Guid> GetAllIds();
        Achievements GetById(Guid id);
        void Create(Achievements input);
        void Update(Achievements input);
        void SoftDelete(Guid id, string username);
    }
}
