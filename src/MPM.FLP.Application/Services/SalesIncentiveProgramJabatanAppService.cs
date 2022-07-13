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
    public class SalesIncentiveProgramJabatanAppService : FLPAppServiceBase, ISalesIncentiveProgramJabatanAppService
    {
        private readonly IRepository<SalesIncentiveProgramJabatans, Guid> _salesIncentiveProgramJabatanRepository;

        public SalesIncentiveProgramJabatanAppService(IRepository<SalesIncentiveProgramJabatans, Guid> salesIncentiveProgramJabatanRepository)
        {
            _salesIncentiveProgramJabatanRepository = salesIncentiveProgramJabatanRepository;
        }

        public void Create(SalesIncentiveProgramJabatans input)
        {
            _salesIncentiveProgramJabatanRepository.Insert(input);
        }

        public void Delete(Guid id)
        {
            var salesIncentiveProgramJabatan = _salesIncentiveProgramJabatanRepository.FirstOrDefault(x => x.Id == id);
            _salesIncentiveProgramJabatanRepository.Delete(salesIncentiveProgramJabatan);
        }

        public IQueryable<SalesIncentiveProgramJabatans> GetAll()
        {
            return _salesIncentiveProgramJabatanRepository.GetAll();
        }

        public SalesIncentiveProgramJabatans GetById(Guid id)
        {
            return _salesIncentiveProgramJabatanRepository.GetAll().FirstOrDefault(x => x.Id == id);
        }
    }
}
