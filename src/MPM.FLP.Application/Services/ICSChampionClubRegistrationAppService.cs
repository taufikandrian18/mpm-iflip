using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface ICSChampionClubRegistrationAppService : IApplicationService
    {
        IQueryable<CSChampionClubRegistrations> GetAll();
        CSChampionClubRegistrations GetById(int year);
        bool IsRegisrationEnabled();
        void Create(CSChampionClubRegistrations input);
        void Update(CSChampionClubRegistrations input);
    }
}
