using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Domain.Repositories;
using MPM.FLP.FLPDb;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class DealerAppService : FLPAppServiceBase, IDealerAppService
    {
        private readonly IRepository<Dealers, string> _dealerRepository;
        private readonly IRepository<DealerH3, string> _dealerRepositoryH3;

        public DealerAppService(IRepository<Dealers, string> dealerRepository, IRepository<DealerH3, string> dealerRepositoryH3)
        {
            _dealerRepository = dealerRepository;
            _dealerRepositoryH3 = dealerRepositoryH3;
        }


        public IQueryable<Dealers> GetAll()
        {
            return _dealerRepository.GetAll();
        }

        public List<string> GetChannel()
        {
            return _dealerRepository.GetAll().GroupBy(x => x.Channel).Select(y => y.First().Channel).ToList();
        }
        

        public List<Dealers>  GetDealers(string channel, string kota)
        {
            
            var query = _dealerRepository.GetAll().ToList();
            if(!String.IsNullOrEmpty(channel)){
               query =  query.Where(x => x.Channel == channel).ToList();
            }

            if (!String.IsNullOrEmpty(kota)){
               query =query.Where(x => x.Kota == kota).ToList();
            }
            return query;
        }
        public List<DealerH3> GetDealersH3(string kota)
        {        
            var query = _dealerRepositoryH3.GetAll().ToList();
            if(!String.IsNullOrEmpty(kota)){
               query =  query.Where(x => x.Kota == kota).ToList();
            }
            return query;
        }

        public List<Dealers> GetDealersBackoffice(string kota)
        {
            if (!string.IsNullOrEmpty(kota))
                return _dealerRepository.GetAll().Where(x => x.Kota == kota).ToList();
            return _dealerRepository.GetAll().ToList();
        }

        public List<string> GetKaresidenanH1()
        {       
            return _dealerRepository.GetAll().Where(x => x.Channel == "H1" && !string.IsNullOrEmpty(x.Karesidenan)).GroupBy(x => x.Karesidenan).Select(y => y.First().Karesidenan).ToList();
        }

         public List<string> GetKaresidenan()
        {       
            return _dealerRepository.GetAll().GroupBy(x => x.Karesidenan).Select(y => y.First().Karesidenan).ToList();
        }


        public List<string> GetKaresidenanHC3(string channel)
        {
            return _dealerRepository.GetAll().Where(x => x.Channel == channel && !string.IsNullOrEmpty(x.NamaKaresidenanHC3)).GroupBy(x => x.NamaKaresidenanHC3).Select(y => y.First().NamaKaresidenanHC3).ToList();
        }

        public List<string> GetKotaH1(string karesidenan)
        {
            return _dealerRepository.GetAll().Where(x => x.Karesidenan == karesidenan && !string.IsNullOrEmpty(x.Kota)).GroupBy(x => x.Kota).Select(y => y.First().Kota).ToList();
        }

        public List<string> GetKotaH2()
        {
            return _dealerRepository.GetAll().Where(x => x.Channel == "H2" && !string.IsNullOrEmpty(x.Kota)).GroupBy(x => x.Kota).Select(y => y.First().Kota).ToList();
        }

        public List<string> GetKotaHC3(string channel, string karesidenan)
        {
            return _dealerRepository.GetAll().Where(x => x.Channel == channel && x.NamaKaresidenanHC3 == karesidenan).GroupBy(x => x.Kota).Select(y => y.First().Kota).ToList();
        }
        public List<string> GetKota()
        {
            return _dealerRepository.GetAll().GroupBy(x => x.Kota).Select(y => y.First().Kota).ToList();
        }
    }
}
