using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Authorization;



namespace MPM.FLP.Services
{
    public interface IDealerAppService : IApplicationService
    {
        IQueryable<Dealers> GetAll();
        List<Dealers> GetDealers(string channel, string kota);
        List<DealerH3> GetDealersH3(string kota);

        List<Dealers> GetDealersBackoffice(string kota);

        //H1
        List<string> GetKaresidenanH1();
        List<string> GetKotaH1(string karesidenan);
        

        //H2
        List<string> GetKotaH2();

        //H3
        List<string> GetChannel();
        List<string> GetKaresidenanHC3(string channel);
        List<string> GetKotaHC3(string channel, string karesidenan);
    }
}
