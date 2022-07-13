using Abp.Application.Services;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IInternalUserAppService : IApplicationService
    {
        IQueryable<InternalUsers> GetAll();
        Task<List<InternalUserDto>> GetAllInternalUsers(string channel);
        IQueryable<InternalUsers> GetInternalUserByChannel(string channel);
        Task<List<ActivitiesInternalUsersDto>> GetInternalUserByChannelIncludeActivity(string channel);
        IQueryable<InternalUsers> GetInternalUserByKota(string channel, string kota);
        IQueryable<InternalUsers> GetInternalUserByKaresidenanH1(string karesidenan);
        IQueryable<InternalUsers> GetInternalUserByKaresidenanHC3(string channel, string karesidenan);
        IQueryable<InternalUsers> GetInternalUserByDealer(string channel, string kodeDealerMPM);
        List<InternalUsers> GetInternalUserByDealerMobile(string channel, string kodeDealerMPM);
        List<InternalUsers> GetDirectSubordinates(int idmpm);
        List<InternalUsers> GetCOESubordinates(int idmpm);
        Task<InternalUserDto> GetById(int idMPM);
        Task Update(UpdateInternalUserDto input);
    }
}
