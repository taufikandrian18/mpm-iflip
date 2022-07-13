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
    public class KotaAppService : FLPAppServiceBase, IKotaAppService
    {
        private readonly IRepository<Kotas> _kotaRepository;

        public KotaAppService(IRepository<Kotas> kotaRepository)
        {
            _kotaRepository = kotaRepository;
        }

        public IQueryable<Kotas> GetAll()
        {
            return _kotaRepository.GetAll();
        }
    }
}
