using AutoMapper;
using Castle.Core.Logging;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using MPM.FLP.Doku.Dto;
using MPM.FLP.MPMWallet;
using MPM.FLP.Repositories;
using MPM.FLP.Utilities;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MPM.FLP.Doku
{
    public class DokuWallet : IMpmWalletRepository
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly DokuSettings _dokuSettings;
        private RestClient newClient;

        public DokuWallet(ILoggerFactory loggerFactory, IMapper mapper, HttpClient httpClient, IOptions<DokuSettings> dokuSettings)
        {
            _logger = loggerFactory.Create(typeof(DokuWallet));
            _mapper = mapper;
            _httpClient = httpClient;
            _dokuSettings = dokuSettings.Value;
        }
        public string GetDokuSetting() 
        {
            return _dokuSettings.WebServiceUrl.SignOn;
        }

        public async Task<MpmWalletSignOnResponse> SignOn()
        {
            try
            {
                _logger.Info("SignOn URL = " + _dokuSettings.WebServiceUrl.SignOn);
                DokuSettings.Systrace = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
                DokuSignOnRequestDto dokuSignOnRequest = new DokuSignOnRequestDto(_dokuSettings);
                string webServiceUrlSignOn = QueryHelpers.AddQueryString(_dokuSettings.WebServiceUrl.SignOn, dokuSignOnRequest.ToDictionary());
                var response = await _httpClient.PostAsync(webServiceUrlSignOn, new StringContent(""));
                if (!response.IsSuccessStatusCode) throw new HttpRequestException("Error connecting to Doku Sign On service. Status Code " + ((int)response.StatusCode).ToString());
                var content = await response.Content.ReadAsStringAsync();

                DokuSignOnResponse mpmWalletSignOnResponse = JsonConvert.DeserializeObject<DokuSignOnResponse>(content);

                // Invalid client id/secret
                if (mpmWalletSignOnResponse.ResponseCode == "3017") throw new DokuException(mpmWalletSignOnResponse.ResponseCode, mpmWalletSignOnResponse.ResponseMessage.En);

                DokuSettings.AccessToken = mpmWalletSignOnResponse.AccessToken;
                return mpmWalletSignOnResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<MpmWalletRegisterResponse> Register(MpmWalletRegisterRequest mpmWalletRegisterRequest)
        {
            try
            {
                
                await SignOn();
                IDictionary<string, string> dictCustomer = mpmWalletRegisterRequest.AsDictionary(prefix: "customer");

                DokuRegisterRequestDto dokuRegisterRequest = new DokuRegisterRequestDto(_dokuSettings, mpmWalletRegisterRequest);
                IDictionary<string, string> dictDokuRegisterRequest = dokuRegisterRequest.AsDictionary(removeNullProperties: true);

                var response = await _httpClient.PostAsync(_dokuSettings.WebServiceUrl.Register, new FormUrlEncodedContent(dictDokuRegisterRequest));
                if (!response.IsSuccessStatusCode) throw new HttpRequestException("Error connecting to Doku Register service. Status Code " + ((int)response.StatusCode).ToString());
                var content = await response.Content.ReadAsStringAsync();

                var dokuRegisterResponse = JsonConvert.DeserializeObject<DokuRegisterResponseDto>(content);
                if (dokuRegisterRequest.AccessToken == null || dokuRegisterResponse.ResponseCode == "3010")
                {
                    await SignOn();
                    dokuRegisterRequest = new DokuRegisterRequestDto(_dokuSettings, mpmWalletRegisterRequest);
                    dictDokuRegisterRequest = dokuRegisterRequest.AsDictionary();
                    dictDokuRegisterRequest = dictDokuRegisterRequest.Union(dictCustomer).ToDictionary(z => z.Key, z => z.Value);

                    response = await _httpClient.PostAsync(_dokuSettings.WebServiceUrl.Balance, new FormUrlEncodedContent(dictDokuRegisterRequest));
                    content = await response.Content.ReadAsStringAsync();
                }
                MpmWalletRegisterResponse mpmWalletRegisterResponse = _mapper.Map<MpmWalletRegisterResponse>(dokuRegisterResponse);

                return mpmWalletRegisterResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<NewMpmRegisterResponse> NewRegister(int idMpm)
        {
            try
            {
               await SignOn();
                DokuRegisterRequestDto dokuRegisterRequest = new DokuRegisterRequestDto(_dokuSettings, idMpm);
                var dokuRegisterResponse = new NewMpmRegisterResponse();
                IDictionary<string, string> dictDokuRegisterRequest = dokuRegisterRequest.AsDictionary(removeNullProperties: true);

                //var response = await _httpClient.PostAsync("https://staging.doku.com/dokupay/h2h/signupweb", new FormUrlEncodedContent(dictDokuRegisterRequest));
                var response = await _httpClient.PostAsync(_dokuSettings.WebServiceUrl.NewRegister, new FormUrlEncodedContent(dictDokuRegisterRequest));
                if (!response.IsSuccessStatusCode) throw new HttpRequestException("Error connecting to Doku Register service. Status Code " + ((int)response.StatusCode).ToString());
                var content = await response.Content.ReadAsStringAsync();

                dokuRegisterResponse = JsonConvert.DeserializeObject<NewMpmRegisterResponse>(content); 
                NewMpmRegisterResponse mpmWalletRegisterResponse = _mapper.Map<NewMpmRegisterResponse>(dokuRegisterResponse);

                return mpmWalletRegisterResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<NewMpmRegisterResponse> CheckToken(int idMpm)
        {
            try
            {
                await SignOn();
                DokuRegisterRequestDto dokuRegisterRequest = new DokuRegisterRequestDto(_dokuSettings, idMpm);
                var dokuRegisterResponse = new NewMpmRegisterResponse();
                IDictionary<string, string> dictDokuRegisterRequest = dokuRegisterRequest.AsDictionary(removeNullProperties: true);

                var response = await _httpClient.PostAsync("https://staging.doku.com/dokupay/h2h/signupweb", new FormUrlEncodedContent(dictDokuRegisterRequest));
                if (!response.IsSuccessStatusCode) throw new HttpRequestException("Error connecting to Doku Register service. Status Code " + ((int)response.StatusCode).ToString());
                var content = await response.Content.ReadAsStringAsync();

                dokuRegisterResponse = JsonConvert.DeserializeObject<NewMpmRegisterResponse>(content);
                NewMpmRegisterResponse mpmWalletRegisterResponse = _mapper.Map<NewMpmRegisterResponse>(dokuRegisterResponse);

                mpmWalletRegisterResponse.ResponseMessage.Id = JsonConvert.SerializeObject(dictDokuRegisterRequest);    
                return mpmWalletRegisterResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<MpmWalletBalanceResponse> Balance(string userId, string walletId)
        {
            try
            {
                await SignOn();
                DokuBalanceRequestDto dokuBalanceRequest = new DokuBalanceRequestDto(_dokuSettings, walletId);
                MpmWalletBalanceResponse mpmWalletBalanceResponse = new MpmWalletBalanceResponse();
                var response = await _httpClient.PostAsync(_dokuSettings.WebServiceUrl.Balance, new FormUrlEncodedContent(dokuBalanceRequest.AsDictionary()));
                if (!response.IsSuccessStatusCode) throw new HttpRequestException("Error connecting to Doku Balance service. Status Code " + ((int)response.StatusCode).ToString());
                var content = await response.Content.ReadAsStringAsync();

                DokuBalanceResponseDto dokuBalanceResponse = JsonConvert.DeserializeObject<DokuBalanceResponseDto>(content);

                if (dokuBalanceResponse.ResponseCode == "3009" || // Invalid AccessToken. Resign-on and retry the request
                    dokuBalanceResponse.ResponseCode == "3010") // AccessToken expired. Resign-on and retry the request
                {
                    await SignOn();
                    dokuBalanceRequest = new DokuBalanceRequestDto(_dokuSettings, walletId);
                    mpmWalletBalanceResponse = new MpmWalletBalanceResponse();
                    response = await _httpClient.PostAsync(_dokuSettings.WebServiceUrl.Balance, new FormUrlEncodedContent(dokuBalanceRequest.AsDictionary()));
                    content = await response.Content.ReadAsStringAsync();
                }

                mpmWalletBalanceResponse = dokuBalanceResponse.MapToMpmWallet();
                return await Task.FromResult(mpmWalletBalanceResponse);
            }
            catch (DokuException ex)
            {
                _logger.Error(string.Format("Unsuccessful response from Doku Balance service. User Id: {0}. Doku Id: {1}. Doku Response: {2}. Message: {3}", userId, walletId, ex.ResponseCode, ex.Message), ex);
                throw ex;
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Error getting Doku Balance. User Id: {0}. Doku Id: {1}. Message: {2}", userId, walletId, ex.Message), ex);
                throw ex;
            }
        }

        public async Task<MpmWalletHistoryResponse> History(string userId, string walletId, string lastRefId, string criteriaStartDate, string criteriaEndDate)
        {
            try
            {
                await SignOn();
                DokuHistoryRequestDto dokuHistoriesRequest = new DokuHistoryRequestDto(_dokuSettings, walletId, lastRefId, criteriaStartDate, criteriaEndDate);
                MpmWalletHistoryResponse mpmWalletHistoriesResponse = new MpmWalletHistoryResponse();
                var response = await _httpClient.PostAsync(_dokuSettings.WebServiceUrl.Histories, new FormUrlEncodedContent(dokuHistoriesRequest.AsDictionary()));
                if (!response.IsSuccessStatusCode) throw new HttpRequestException("Error connecting to Doku Activities service. Status Code " + ((int)response.StatusCode).ToString());
                var content = await response.Content.ReadAsStringAsync();

                DokuHistoryResponseDto dokuHistoriesResponse = JsonConvert.DeserializeObject<DokuHistoryResponseDto>(content);

                if (dokuHistoriesResponse.ResponseCode == "3009" || // Invalid AccessToken. Resign-on and retry the request
                    dokuHistoriesResponse.ResponseCode == "3010") // AccessToken expired. Resign-on and retry the request
                {
                    await SignOn();
                    dokuHistoriesRequest = new DokuHistoryRequestDto(_dokuSettings, walletId, lastRefId, criteriaStartDate, criteriaEndDate);
                    mpmWalletHistoriesResponse = new MpmWalletHistoryResponse();
                    response = await _httpClient.PostAsync(_dokuSettings.WebServiceUrl.Histories, new FormUrlEncodedContent(dokuHistoriesRequest.AsDictionary()));
                    content = await response.Content.ReadAsStringAsync();
                }

                mpmWalletHistoriesResponse = dokuHistoriesResponse.MapToMpmWallet();
                return await Task.FromResult(mpmWalletHistoriesResponse);
            }
            catch (DokuException ex)
            {
                _logger.Error(string.Format("Unsuccessful response from Doku Activities service. User Id: {0}. Doku Id: {1}. Doku Response: {2}. Message: {3}", userId, walletId, ex.ResponseCode, ex.Message), ex);
                throw ex;
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Error getting Doku Activities. User Id: {0}. Doku Id: {1}. Message: {2}", userId, walletId, ex.Message), ex);
                throw ex;
            }
        }
    }
}
