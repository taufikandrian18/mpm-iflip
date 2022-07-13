using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IRolePlayAssignmentAppService : IApplicationService
    {
        IQueryable<RolePlayAssignments> GetAll();
        RolePlayAssignments GetById(Guid id);
        void Create(RolePlayAssignments input);
        void Delete(Guid id);
    }
}
