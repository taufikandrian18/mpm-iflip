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
    public class RolePlayDetailAppService : FLPAppServiceBase, IRolePlayDetailAppService
    {
        private readonly IRepository<RolePlayDetails, Guid> _rolePlayDetailRepository;

        public RolePlayDetailAppService(IRepository<RolePlayDetails, Guid> rolePlayDetailRepository)
        {
            _rolePlayDetailRepository = rolePlayDetailRepository;
        }

        public void Create(RolePlayDetails input)
        {
            _rolePlayDetailRepository.Insert(input);
        }

        public IQueryable<RolePlayDetails> GetAll()
        {
            return _rolePlayDetailRepository.GetAll();
        }

        public List<RolePlayDetails> GetAllItemByRolePlay(Guid rolePlayId)
        {
            return _rolePlayDetailRepository.GetAll().Where(x => x.RolePlayId == rolePlayId).ToList();
        }

        public RolePlayDetails GetById(Guid id)
        {
            return _rolePlayDetailRepository.GetAll().Where(x=> x.DeletionTime == null).FirstOrDefault(x => x.Id == id);
        }

        public void SoftDelete(Guid id, string username)
        {
            var rolePlayDetail = _rolePlayDetailRepository.FirstOrDefault(x => x.Id == id);
            rolePlayDetail.DeleterUsername = username;
            rolePlayDetail.DeletionTime = DateTime.Now;
            _rolePlayDetailRepository.Update(rolePlayDetail);
        }

        public void Update(RolePlayDetails input)
        {
            _rolePlayDetailRepository.Update(input);
        }
    }
}
