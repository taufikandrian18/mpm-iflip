using Abp.Domain.Repositories;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public class DealerH3AppService : FLPAppServiceBase, IDealerH3AppService
    {
        private readonly ILogger _logger;
        private readonly IRepository<DealerH3, string> _repository;

        public DealerH3AppService(ILogger logger, IRepository<DealerH3, string> repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public BaseResponse GetAll([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _repository.GetAll().Where(x => x.DeletionTime == null);
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Nama.Contains(request.Query));
            }
            var count = query.Count();

            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        public DealerH3 GetById(string id)
        {
            return _repository.FirstOrDefault(id);
        }

        public void Create(DealerH3 input)
        {
            _repository.Insert(input);
        }

        public void Update(DealerH3 input)
        {
            _repository.Update(input);
        }
        
        public void SoftDelete(string id, string username)
        {
            var sekolah = _repository.Get(id);
            sekolah.DeleterUsername = username;
            sekolah.DeletionTime = DateTime.Now;

            _repository.Update(sekolah);
        }

        public async Task<ServiceResult> Sync()
        {
            try
            {
                var key = await AppHelpers.MPMLogin();
                var url = string.Format(AppConstants.MPMDealerUrl, "2");

                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(key);
                client.DefaultRequestHeaders.Add("User", AppConstants.MpmLoginUsername);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var getDealerResult = await client.GetAsync(url);
                var dealerJson = await getDealerResult.Content.ReadAsStringAsync();

                DealerH3SyncResponseDto dealerResponse = JsonConvert.DeserializeObject<DealerH3SyncResponseDto>(dealerJson);
                if (dealerResponse.status == 1)
                {
                    //Sync Delete
                    var deletedDealer = _repository.GetAll().Where(x => x.DeletionTime == null 
                        && !dealerResponse.data.Select(y => y.accountnum).Contains(x.AccountNumber)).ToList();
                    foreach(var dealer in deletedDealer)
                    {
                        dealer.DeleterUsername = "system";
                        dealer.DeletionTime = DateTime.Now;
                        _repository.Update(dealer);
                    }

                    //Sync Insert and Update
                    foreach (var dealer in dealerResponse.data)
                    {
                        var _dealer = ObjectMapper.Map<DealerH3>(dealer);
                        _repository.InsertOrUpdate(_dealer);
                    }
                    return new ServiceResult { IsSuccess = true, Message = "Sync Success" };
                }
                else
                {
                    return new ServiceResult { IsSuccess = false, Message = dealerResponse.message };
                }
            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message);
                return new ServiceResult { IsSuccess = false, Message = ex.Message };
            }
        }
    }
}
