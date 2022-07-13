using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IRolePlayDetailAppService : IApplicationService
    {
        IQueryable<RolePlayDetails> GetAll();
        List<RolePlayDetails> GetAllItemByRolePlay(Guid rolePlayId);
        RolePlayDetails GetById(Guid id);
        void Create(RolePlayDetails input);
        void Update(RolePlayDetails input);
        void SoftDelete(Guid id, string username);
    }
}
