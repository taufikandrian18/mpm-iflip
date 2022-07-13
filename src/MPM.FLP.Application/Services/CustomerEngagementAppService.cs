using Abp.Authorization;
using Abp.Domain.Repositories;
using Castle.Core.Logging;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class CustomerEngagementAppService : FLPAppServiceBase, ICustomerEngagementAppService
    {
        private readonly ILogger _logger;
        private readonly IRepository<Kotas, int> _kotaRepository;
        private readonly IRepository<Agamas, string> _agamaRepository;
        private readonly IRepository<Pekerjaans, string> _pekerjaanRepository;
        public CustomerEngagementAppService(ILoggerFactory loggerFactory, 
                                            IRepository<Kotas, int> kotaRepository, 
                                            IRepository<Agamas, string> agamaRepository, 
                                            IRepository<Pekerjaans, string> pekerjaanRepository)
        {
            _logger = loggerFactory.Create(typeof(CustomerEngagementAppService));
            _kotaRepository = kotaRepository;
            _agamaRepository = agamaRepository;
            _pekerjaanRepository = pekerjaanRepository;
        }
        public CustomerEngagementOptionDto GetCustomerEngagementOption()
        {
            CustomerEngagementOptionDto options = new CustomerEngagementOptionDto();
            options.Kota = _kotaRepository.GetAll().Select(x => new KotaDto() 
            {
                CountyId = x.CountyId,
                Kota = x.NamaKota,
                Kecamatan = x.Kecamatans.Select(y => y.NamaKecamatan).ToList()
            }).ToList();
            options.Agama = _agamaRepository.GetAll().OrderBy(x => x.CreationTime).Select(x => x.NamaAgama).ToList();
            options.Pekerjaan = _pekerjaanRepository.GetAll().Select(x => x.NamaPekerjaan).ToList();
            return options;
        }

        public async Task<List<CustomerEngagementDataDto>> EngageCustomer(CustomerEngagementSubmitDto input)
        {
            var result = new List<CustomerEngagementDataDto>();
            try
            {
                var key = await AppHelpers.MPMLogin();
                //var url = "https://api.mpm-motor.com/marketingv2/flp/getcustomer?KOTA=3506&KECAMATAN=%&BLNLAHIR=%&FLPSALES=36033&AGAMA=%&PEKERJAAN=%&TGLISINAMA1=2020-02-01&TGLISINAMA2=2020-02-26";

                var url = AppConstants.MpmCustomerUrl;
                var paramKota = "?KOTA=" + input.CountyId;
                var paramKecamatan = "&KECAMATAN=" + (string.IsNullOrEmpty(input.Kecamatan) ? "%" : HttpUtility.UrlEncode(input.Kecamatan));
                var paramBulanLahir = "&BLNLAHIR=" + (input.BulanLahir == 0 ? "%" : input.BulanLahir.ToString());
                var paramFLPSales = "&FLPSALES=" + input.FLPId;
                var paramAgama = "&AGAMA=" + (string.IsNullOrEmpty(input.Agama) ? "%" : input.Agama);
                var paramPekerjaan = "&PEKERJAAN=" + (string.IsNullOrEmpty(input.Pekerjaan) ? "%" : HttpUtility.UrlEncode(input.Pekerjaan));
                var paramTglBeliAwal = "&TGLISINAMA1=" + input.TanggalBeliMulai.ToString("yyyy-MM-dd");
                var paramTglBeliAkhir = "&TGLISINAMA2=" + input.TanggalBeliTerakhir.ToString("yyyy-MM-dd");
                url = url + paramKota + paramKecamatan + paramBulanLahir + paramFLPSales + paramAgama + paramPekerjaan + paramTglBeliAwal + paramTglBeliAkhir;

                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(key);
                client.DefaultRequestHeaders.Add("User", AppConstants.MpmLoginUsername);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var getCustomerResult = await client.GetAsync(url);
                var customerJson = await getCustomerResult.Content.ReadAsStringAsync();

                CustomerEngagementResponseDto customerResponse = JsonConvert.DeserializeObject<CustomerEngagementResponseDto>(customerJson);
                result = customerResponse.data;
            }
            catch (Exception e) { _logger.Error(e.Message + "\n" + e.InnerException.Message); }
            return result;
        }
    }
}
