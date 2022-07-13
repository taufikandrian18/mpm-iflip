using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class CSChampionClubParticipantAppService : FLPAppServiceBase, ICSChampionClubParticipantAppService
    {
        private readonly IRepository<CSChampionClubParticipants, Guid> _csChampionClubParticipantRepository;
        private readonly IRepository<CSChampionClubRegistrations> _csChampionClubRegistrationRepository;

        public CSChampionClubParticipantAppService(IRepository<CSChampionClubParticipants, Guid> csChampionClubParticipantRepository,
                                                   IRepository<CSChampionClubRegistrations> csChampionClubRegistrationRepository)
        {
            _csChampionClubParticipantRepository = csChampionClubParticipantRepository;
            _csChampionClubRegistrationRepository = csChampionClubRegistrationRepository;
        }

        public IQueryable<CSChampionClubParticipants> GetAll()
        {
            return _csChampionClubParticipantRepository.GetAll().Include(y => y.InternalUser);
        }

        public CSChampionClubParticipants GetById(Guid id)
        {
            var csChampionClubParticipant = _csChampionClubParticipantRepository.FirstOrDefault(x => x.Id == id);
            return csChampionClubParticipant;
        }

        public List<CSChampionClubParticipants> GetByUser(int idMPM)
        {
            var csChampionClubParticipants = _csChampionClubParticipantRepository.GetAllIncluding(x => x.InternalUser)
                    .Where(x => x.IDMPM == idMPM).ToList();
            return csChampionClubParticipants;
        }

        public bool IsAbleToRegister(int idmpm)
        {
            var now = DateTime.Now;
            var participant = _csChampionClubParticipantRepository.GetAll()
                        .FirstOrDefault(x => x.IDMPM == idmpm && x.Year == now.Year);
            if (participant != null)
                return false;

            var registration = _csChampionClubRegistrationRepository.GetAll().FirstOrDefault(x => x.Year == now.Year);
            if (now.Date >= registration.StartDate.Date && now.Date <= registration.EndDate.Date)
                return true;
            else
                return false;
        }

        public void Register(CSChampionClubParticipantregisterDto input)
        {
            CSChampionClubParticipants newParticipant = new CSChampionClubParticipants() 
            {
                Id = Guid.NewGuid(),
                IDMPM = input.IDMPM,
                Year = DateTime.UtcNow.AddHours(7).Year,
                CreatorUsername = "System",
                CreationTime = DateTime.UtcNow.AddHours(7)
            };
            _csChampionClubParticipantRepository.Insert(newParticipant);
        }

        public void SoftDelete(Guid id, string username)
        {
            var csChampionClubParticipant = _csChampionClubParticipantRepository.FirstOrDefault(x => x.Id == id);
            csChampionClubParticipant.DeleterUsername = username;
            csChampionClubParticipant.DeletionTime = DateTime.Now;
            _csChampionClubParticipantRepository.Update(csChampionClubParticipant);
        }
    }
}
