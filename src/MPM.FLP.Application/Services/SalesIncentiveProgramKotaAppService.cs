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
    public class SalesIncentiveProgramKotaAppService : FLPAppServiceBase, ISalesIncentiveProgramKotaAppService
    {
        private readonly IRepository<SalesIncentiveProgramKotas, Guid> _salesIncentiveProgramKotaRepository;

        public SalesIncentiveProgramKotaAppService(IRepository<SalesIncentiveProgramKotas, Guid> salesIncentiveProgramKotaRepository)
        {
            _salesIncentiveProgramKotaRepository = salesIncentiveProgramKotaRepository;
        }

        public void Create(SalesIncentiveProgramKotas input)
        {
            _salesIncentiveProgramKotaRepository.Insert(input);
        }

        public void Delete(Guid id)
        {
            var salesIncentiveProgramKota = _salesIncentiveProgramKotaRepository.FirstOrDefault(x => x.Id == id);
            _salesIncentiveProgramKotaRepository.Delete(salesIncentiveProgramKota);
        }

        public IQueryable<SalesIncentiveProgramKotas> GetAll()
        {
            return _salesIncentiveProgramKotaRepository.GetAll();
        }

        public SalesIncentiveProgramKotas GetById(Guid id)
        {
            return _salesIncentiveProgramKotaRepository.GetAll().FirstOrDefault(x => x.Id == id);
        }
    }
}
