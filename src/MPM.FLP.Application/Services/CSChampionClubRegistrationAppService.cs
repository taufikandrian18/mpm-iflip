using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public class CSChampionClubRegistrationAppService : FLPAppServiceBase, ICSChampionClubRegistrationAppService
    {
        private readonly IRepository<CSChampionClubRegistrations> _csChampionClubRegistrationRepository;
        private readonly IRepository<CSChampionClubParticipants, Guid> _csChampionClubParticipantRepository;
        private readonly IRepository<InternalUsers> _internalUserRepository;
        private readonly IAbpSession _abpSession;

        public CSChampionClubRegistrationAppService(IRepository<CSChampionClubRegistrations> csChampionClubRegistrationRepository,
                                      IRepository<CSChampionClubParticipants, Guid> csChampionClubParticipantRepository,
                                      IAbpSession abpSession,
                                      IRepository<InternalUsers> internalUserRepository)
        {
            _csChampionClubRegistrationRepository = csChampionClubRegistrationRepository;
            _csChampionClubParticipantRepository = csChampionClubParticipantRepository;
            _internalUserRepository = internalUserRepository;
            _abpSession = abpSession;
        }
        public void Create(CSChampionClubRegistrations input)
        {
            _csChampionClubRegistrationRepository.Insert(input);
        }

        public IQueryable<CSChampionClubRegistrations> GetAll()
        {
            return _csChampionClubRegistrationRepository.GetAll();
        }

        public CSChampionClubRegistrations GetById(int year)
        {
            return _csChampionClubRegistrationRepository.GetAll().FirstOrDefault(x => x.Year == year);
        }

        public bool IsRegisrationEnabled()
        {
            try
            {

                long currentUserId = _abpSession.UserId.Value;
                var internalUser = _internalUserRepository.GetAll().FirstOrDefault(x => x.AbpUserId.Value == currentUserId);
                if (internalUser != null)
                {
                    DateTime today = DateTime.UtcNow.AddHours(7);
                    int year = today.Year;
                    var registration = _csChampionClubRegistrationRepository.GetAll().FirstOrDefault(x => x.Year == year);
                    
                    if (registration == null) 
                        return false;

                    if (registration.StartDate.Date >= today && registration.EndDate <= today)
                        return true;
                }
                return false;
            }
            catch (Exception)
            { return false; }
        }

        public void Update(CSChampionClubRegistrations input)
        {
            _csChampionClubRegistrationRepository.Update(input);
        }
    }
}
