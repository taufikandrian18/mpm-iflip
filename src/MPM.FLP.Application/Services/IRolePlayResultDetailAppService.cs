using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IRolePlayResultDetailAppService : IApplicationService
    {
        IQueryable<RolePlayResultDetails> GetAll();
        RolePlayResultDetails GetById(Guid id);
        void Create(RolePlayResultDetails input);
        void Update(RolePlayResultDetails input);
        void SoftDelete(Guid id, string username);
    }
}
