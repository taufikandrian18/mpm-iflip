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
    public class RolePlayResultDetailAppService : FLPAppServiceBase, IRolePlayResultDetailAppService
    {
        private readonly IRepository<RolePlayResultDetails, Guid> _rolePlayResultDetailRepository;

        public RolePlayResultDetailAppService(IRepository<RolePlayResultDetails, Guid> rolePlayResultDetailRepository)
        {
            _rolePlayResultDetailRepository = rolePlayResultDetailRepository;
        }

        public void Create(RolePlayResultDetails input)
        {
            _rolePlayResultDetailRepository.Insert(input);
        }

        public IQueryable<RolePlayResultDetails> GetAll()
        {
            return _rolePlayResultDetailRepository.GetAll();
        }

        public RolePlayResultDetails GetById(Guid id)
        {
            return _rolePlayResultDetailRepository.GetAll().FirstOrDefault(x => x.Id == id);
        }

        public void SoftDelete(Guid id, string username)
        {
            var rolePlayResultDetail = _rolePlayResultDetailRepository.FirstOrDefault(x => x.Id == id);
            rolePlayResultDetail.DeleterUsername = username;
            rolePlayResultDetail.DeletionTime = DateTime.Now;
            _rolePlayResultDetailRepository.Update(rolePlayResultDetail);
        }

        public void Update(RolePlayResultDetails input)
        {
            _rolePlayResultDetailRepository.Update(input);
        }
    }
}
