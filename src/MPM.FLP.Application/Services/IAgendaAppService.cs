using Abp.Application.Services;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IAgendaAppService : IApplicationService
    {
        Task<ServiceResult> Create(AgendaCreateDto input);
        ServiceResult Assign(AgendaAssignDto input);
        ServiceResult UnAssign(AgendaDeleteDto input);
        List<Agendas> GetAgendaByUser(int idmpm);
    }
}
