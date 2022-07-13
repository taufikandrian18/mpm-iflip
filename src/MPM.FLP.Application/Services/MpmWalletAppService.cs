using Abp;
using Abp.Authorization;
using Abp.Domain.Uow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MPM.FLP.Authorization.Users;
using MPM.FLP.FLPDb;
using MPM.FLP.MPMWallet;
using MPM.FLP.Repositories;
using MPM.FLP.Services.Dto;
using MPM.FLP.Services.Dto.MpmWallet;
using MPM.FLP.Users;
using MPM.FLP.Users.Dto;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public class MpmWalletAppService : FLPAppServiceBase, IMpmWalletAppService
    {
        private readonly IMpmWalletRepository _mpmWalletRepository;
        private readonly IUserAppService _userAppService;
        private readonly IInternalUserAppService _internalUserAppService;
        private readonly IExternalUserAppService _externalUserAppService;

        public MpmWalletAppService(IMpmWalletRepository mpmWalletRepository, IUserAppService userAppService, IInternalUserAppService internalUserAppService, IExternalUserAppService externalUserAppService)
        {
            _mpmWalletRepository = mpmWalletRepository;
            _userAppService = userAppService;
            _internalUserAppService = internalUserAppService;
            _externalUserAppService = externalUserAppService;
        }

        public string GetDokuSetting()
        {
            return _mpmWalletRepository.GetDokuSetting();
        }

        public async Task<MpmWalletRegisterResponseDto> Register()
        {
            var currentUser = await GetCurrentUserAsync();
            return await Register(currentUser);
        }

        private async Task<MpmWalletRegisterResponseDto> Register(User user)
        {
            User currentUser = user;
            InternalUsers internalUser = null;
            ExternalUserDto externalUserDto = null;
            MpmWalletRegisterRequest mpmWalletRegisterRequest;

            internalUser = _internalUserAppService.GetAll().Where(x => x.AbpUserId == currentUser.Id).FirstOrDefault();

            if (internalUser != null)
            {
                mpmWalletRegisterRequest = new MpmWalletRegisterRequest(internalUser);
            }
            else
            {
                externalUserDto = await _externalUserAppService.GetByAbpId(currentUser.Id);
                ExternalUsers externalUser = ObjectMapper.Map<ExternalUsers>(externalUserDto);
                if (externalUser == null) throw new AbpException("User Not Found");
                mpmWalletRegisterRequest = new MpmWalletRegisterRequest(currentUser.EmailAddress, externalUser);
            }

            MpmWalletRegisterRequest request = ObjectMapper.Map<MpmWalletRegisterRequest>(mpmWalletRegisterRequest);
            MpmWalletRegisterResponse response = await _mpmWalletRepository.Register(request);
            if (currentUser.WalletId != response.WalletId)
            {
                currentUser.WalletId = response.WalletId;
                UserDto userDto = ObjectMapper.Map<UserDto>(currentUser);
                await _userAppService.Update(userDto);
            }
            return ObjectMapper.Map<MpmWalletRegisterResponseDto>(response);
        }

        public async Task<MpmWalletBalanceResponseDto> Balance()
        {
            var currentUser = await GetCurrentUserAsync();
            string walletId = currentUser.WalletId;
            MpmWalletBalanceResponse response;
            if (string.IsNullOrEmpty(walletId))
            {
                response = MpmWalletBalanceResponseDto.NullWalletId();
            }
            else
            {
                response = await _mpmWalletRepository.Balance(currentUser.UserName, walletId);
            }
            return ObjectMapper.Map<MpmWalletBalanceResponseDto>(response);
        }

        public async Task<MpmWalletHistoryResponseDto> History(string lastRefId, string criteriaStartDate, string criteriaEndDate)
        {
            var currentUser = await GetCurrentUserAsync();
            string walletId = currentUser.WalletId;
            MpmWalletHistoryResponse response;
            if (string.IsNullOrEmpty(walletId))
            {
                response = MpmWalletHistoryResponseDto.NullWalletId();
            }
            else
            {
                response = await _mpmWalletRepository.History(currentUser.UserName, walletId, lastRefId, criteriaStartDate, criteriaEndDate);
            }
            return ObjectMapper.Map<MpmWalletHistoryResponseDto>(response);
        }

        public async Task<NewMpmRegisterResponse> NewRegister()
        {
            NewMpmRegisterResponse response = new NewMpmRegisterResponse();
            MpmWalletResponseMessage detail = new MpmWalletResponseMessage();
            var currentUser = await GetCurrentUserAsync();
            var internalUser = _internalUserAppService.GetAll().Where(x => x.AbpUserId == currentUser.Id).FirstOrDefault();


            if (internalUser != null)
            {
                response = await _mpmWalletRepository.NewRegister(internalUser.IDMPM);
            }
            else
            {
                response.ResponseCode = "404";
                detail.Id = "User Tidak Terdaftar";
                detail.En = "User Not Found";
                response.ResponseMessage = detail;
            }


            return ObjectMapper.Map<NewMpmRegisterResponse>(response);
        }

        public async Task<NewMpmRegisterResponse> CheckToken()
        {
            NewMpmRegisterResponse response = new NewMpmRegisterResponse();
            var currentUser = await GetCurrentUserAsync();
            var internalUser = _internalUserAppService.GetAll().Where(x => x.AbpUserId == currentUser.Id).FirstOrDefault();


            if (internalUser != null)
            {
                response = await _mpmWalletRepository.CheckToken(internalUser.IDMPM);
            }
            else
            {
                response.ResponseCode = "404";
                response.ResponseMessage.Id = "User Tidak Terdaftar";
                response.ResponseMessage.En = "User Not Found";
            }


            return ObjectMapper.Map<NewMpmRegisterResponse>(response);
        }

        public async Task<UserResponse> Check()
        {
            UserResponse response = new UserResponse();
            response.Code = "200";
            var currentUser = await GetCurrentUserAsync();
            var internalUser = _internalUserAppService.GetAll().Where(x => x.AbpUserId == currentUser.Id).FirstOrDefault();
           
            if (internalUser == null)
            {
                response.WalletId = " ";
            }
            else
            {
                var temp = await GettUserByIdAbpAsync(internalUser.AbpUserId.ToString());
                response.WalletId = temp.WalletId;
            }



            return ObjectMapper.Map<UserResponse>(response);
        }

        [HttpGet]
        [AllowAnonymous]
        [AbpAllowAnonymous]
        public async Task Callback(int IdMpm, string AccountId)
        {
            var internalUser = _internalUserAppService.GetAll().FirstOrDefault(x => x.IDMPM == IdMpm);
            var user = GettUserByIdAbpAsync(internalUser.AbpUserId.ToString());

            string toBeSearched = "?accountId=";
            int ix = AccountId.IndexOf(toBeSearched);

            if (ix != -1)
            {
                AccountId = AccountId.Substring(ix + toBeSearched.Length);
            }

            user.Result.WalletId = AccountId.ToString();
            UserDto userDto = ObjectMapper.Map<UserDto>(user.Result);
            await _userAppService.UpdateWallet(userDto);
        }

    }
}
