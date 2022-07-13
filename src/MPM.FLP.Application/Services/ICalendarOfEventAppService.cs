using Abp.Application.Services;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface ICalendarOfEventAppService : IApplicationService
    {
        void Create(CalendarOfEventCreateDto input);
        void Assign(CalendarOfEventAssignDto input);
        void UnAssign(CalendarOfEventDeleteDto input);
        List<Events> GetEventByUser(int idmpm);
        Task<List<MPMEventDto>> GetEventMPMAsync(string kodeDealer);
    }
}
