using Abp.Application.Services;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface ICSChampionClubParticipantAppService : IApplicationService
    {
        IQueryable<CSChampionClubParticipants> GetAll();
        CSChampionClubParticipants GetById(Guid id);
        List<CSChampionClubParticipants> GetByUser(int idMPM);
        bool IsAbleToRegister(int idmpm);
        void Register(CSChampionClubParticipantregisterDto input);
        void SoftDelete(Guid input, string username);
    }
}
