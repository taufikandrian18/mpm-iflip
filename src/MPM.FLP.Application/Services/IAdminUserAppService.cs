using Abp.Application.Services;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IAdminUserAppService : IApplicationService
    {
        Task<List<AdminUserDto>> GetAll();
        Task<AdminUserDto> GetById(Guid id);
        Task Create(CreateAdminUserDto input);
        Task Update(AdminUserDto input);
        string GetChannel();
    }
}
