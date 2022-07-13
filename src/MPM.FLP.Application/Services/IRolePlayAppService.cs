using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IRolePlayAppService : IApplicationService
    {
        IQueryable<RolePlays> GetAll();
        List<RolePlays> GetAllItems();
        RolePlays GetById(Guid id);
        void Create(RolePlays input);
        void Update(RolePlays input);
        void SoftDelete(Guid id, string username);
    }
}
