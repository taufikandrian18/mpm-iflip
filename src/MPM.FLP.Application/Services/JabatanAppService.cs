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
    public class JabatanAppService : FLPAppServiceBase, IJabatanAppService
    {
        private readonly IRepository<Jabatans> _jabatanRepository;

        public JabatanAppService(IRepository<Jabatans> jabatanRepository)
        {
            _jabatanRepository = jabatanRepository;
        }

        public IQueryable<Jabatans> GetAll()
        {
            return _jabatanRepository.GetAll();
        }
    }
}
