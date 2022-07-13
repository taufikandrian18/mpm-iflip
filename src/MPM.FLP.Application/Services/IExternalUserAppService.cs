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
    public interface IExternalUserAppService : IApplicationService
    {
        Task<List<ExternalUserDto>> GetAll();
        Task<ExternalUserDto> GetById(Guid id);
        Task<ExternalUserDto> GetByAbpId(long id);
        Task Update(UpdateExternalUserDto input);
    }
}
