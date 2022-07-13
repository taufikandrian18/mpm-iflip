using Abp.Authorization;
using Abp.Domain.Repositories;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class RolePlayAssignmentAppService : FLPAppServiceBase, IRolePlayAssignmentAppService
    {
        private readonly IRepository<RolePlayAssignments, Guid> _rolePlayAssignmentRepository;

        public RolePlayAssignmentAppService(IRepository<RolePlayAssignments, Guid> rolePlayAssignmentRepository)
        {
            _rolePlayAssignmentRepository = rolePlayAssignmentRepository;
        }

        public void Create(RolePlayAssignments input)
        {
            _rolePlayAssignmentRepository.Insert(input);
        }

        public IQueryable<RolePlayAssignments> GetAll()
        {
            return _rolePlayAssignmentRepository.GetAll();
        }
        
        public RolePlayAssignments GetById(Guid id)
        {
            return _rolePlayAssignmentRepository.GetAll().FirstOrDefault(x => x.Id == id);
        }

        public void Delete(Guid id)
        {
            _rolePlayAssignmentRepository.Delete(id);
        }
    }
}
