using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Castle.Core.Logging;
using iTextSharp.text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Authorization;
using MPM.FLP.Authorization.Users;
using MPM.FLP.FLPDb;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using MPM.FLP.Sms;
using MPM.FLP.Users;
using MPM.FLP.Users.Dto;
using Newtonsoft.Json;
using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{

    public class MobileAccountAppService : FLPAppServiceBase, IMobileAccountAppService
    {
        private readonly ILogger _logger;
        private readonly IRepository<ExternalUsers, Guid> _externalUserRepository;
        private readonly IRepository<InternalUsers> _internalUserRepository;
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly UserManager _userManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRepository<Articles, Guid> _articleRepository;
        private readonly IRepository<BrandCampaigns, Guid> _brandCampaignRepository;
        private readonly IRepository<SalesTalks, Guid> _salesTalkRepository;
        private readonly IRepository<ServiceTalkFlyers, Guid> _serviceTalkFlyerRepository;
        private readonly IUserAppService _userAppService;
        private readonly IRepository<CSChampionClubParticipants, Guid> _csChampionClubParticipantRepository;
        private readonly IRepository<CSChampionClubRegistrations> _csChampionClubRegistrationRepository;
        private readonly IRepository<Sekolahs, Guid> _sekolahRepository;
        private readonly IRepository<TBSMUserGurus, Guid> _tbsmGuruRepository;
        private readonly IRepository<TBSMUserSiswas, Guid> _tbsmSiswaRepository;

        public MobileAccountAppService(
            ILoggerFactory loggerFactory,
            IRepository<ExternalUsers, Guid> externalUserRepository,
            IRepository<InternalUsers> internalUserRepository,
            UserRegistrationManager userRegistrationManager,
            UserManager userManager,
            IPasswordHasher<User> passwordHasher,
            IRepository<Articles, Guid> articleRepository,
            IRepository<BrandCampaigns, Guid> brandCampaignRepository,
            IRepository<SalesTalks, Guid> salesTalkRepository,
            IRepository<ServiceTalkFlyers, Guid> serviceTalkFlyerRepository, 
            IUserAppService userAppService,
            IRepository<CSChampionClubParticipants, Guid> csChampionClubParticipantRepository,
            IRepository<CSChampionClubRegistrations> csChampionClubRegistrationRepository,
            IRepository<Sekolahs, Guid> sekolahRepository,
            IRepository<TBSMUserGurus, Guid> tbsmGuruRepository,
            IRepository<TBSMUserSiswas, Guid> tbsmSiswaRepository
        )
        {
            _logger = loggerFactory.Create(typeof(MobileAccountAppService));
            _externalUserRepository = externalUserRepository;
            _internalUserRepository = internalUserRepository;
            _userRegistrationManager = userRegistrationManager;
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _articleRepository = articleRepository;
            _brandCampaignRepository = brandCampaignRepository;
            _salesTalkRepository = salesTalkRepository;
            _serviceTalkFlyerRepository = serviceTalkFlyerRepository;
            _userAppService = userAppService;
            _csChampionClubParticipantRepository = csChampionClubParticipantRepository;
            _csChampionClubRegistrationRepository = csChampionClubRegistrationRepository;
            _sekolahRepository = sekolahRepository;
            _tbsmGuruRepository = tbsmGuruRepository;
            _tbsmSiswaRepository = tbsmSiswaRepository;
        }

        public ServiceResult CheckUsername(string username)
        {
            var user = _externalUserRepository.GetAll().FirstOrDefault(x => x.UserName.ToLower() == username.ToLower());

            bool isSuccess;
            string message;

            if (user != null) { isSuccess = false; message = "username already exist"; }
            else { isSuccess = true; message = "username is available"; }

            return new ServiceResult() { IsSuccess = isSuccess, Message = message };
        }

        public ServiceResult ConfirmOTP(ConfirmOTPDto input)
        {
            try
            {
                var base32Bytes = Base32Encoding.ToBytes(input.Key);
                var totp = new Totp(base32Bytes, totpSize: 4, step: 86400);

                bool isSuccess = totp.VerifyTotp(input.Code, out long time);
                string message = isSuccess ? string.Empty : "Kode OTP Tidak valid";

                return new ServiceResult() { IsSuccess = isSuccess, Message = message };
            }
            catch (Exception e) 
            {
                _logger.Error(e.Message);
                return new ServiceResult() { IsSuccess = false, Message = "Gagal mengkonfirm kode OTP" };
            }
        }

        public async Task<ServiceResult> ExternalUserRegister(RegisterExternalUserDto input)
        {
            try
            {
                var existingUser = _externalUserRepository.GetAll().FirstOrDefault(x => x.Handphone == input.Handphone);
                if (existingUser != null)
                { return new ServiceResult { IsSuccess = false, Message = "Phone Number already used" }; }

                // var user = await _userRegistrationManager.RegisterMobileAsync(
                //    input.Name,
                //    input.Name,
                //    input.Email,
                //    input.Username,
                //    input.Password,
                //    true
                //);
                var user = await _userRegistrationManager.RegisterAsync(
                    input.Name,
                    input.Name,
                    input.Email,
                    input.Username,
                    input.Password,
                    true
                ); 

                var externalUser = new ExternalUsers()
                {
                    UserName = input.Username,
                    Name = input.Name,
                    Channel = "H3",
                    AbpUserId = user.Id,
                    Address = input.Address,
                    ShopName = input.ShopName,
                    ShopImageurl = input.ShopImageurl,
                    KTPImageUrl = input.KTPImageUrl,
                    UserImageUrl = input.UserImageUrl,
                    Handphone = input.Handphone,
                    Latitude = input.Longitude,
                    Longitude = input.Longitude,
                    Jabatan = input.Jabatan,
                    CreatorUsername = input.Username,
                    CreationTime = DateTime.UtcNow.AddHours(7),
                };

                _externalUserRepository.Insert(externalUser);

                return new ServiceResult { IsSuccess = true, Message = "Registration Success" };
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return new ServiceResult { IsSuccess = false, Message = e.Message }; 
            }

        }

        public async Task<ServiceResult> ExternalUserRegisterWithUpload([FromForm]RegisterWithUploadExternalUserDto input)
        {
            try
            {
                var existingUser = _externalUserRepository.GetAll().FirstOrDefault(x => x.Handphone == input.Handphone);
                if (existingUser != null)
                { return new ServiceResult { IsSuccess = false, Message = "Phone Number already used" }; }

                // var user = await _userRegistrationManager.RegisterMobileAsync(
                //    input.Name,
                //    input.Name,
                //    input.Email,
                //    input.Username,
                //    input.Password,
                //    true
                //);
                var user = await _userRegistrationManager.RegisterAsync(
                    input.Name,
                    input.Name,
                    input.Email,
                    input.Username,
                    input.Password,
                    true
                );

                var externalUser = new ExternalUsers()
                {
                    UserName = input.Username,
                    Name = input.Name,
                    Channel = "H3",
                    AbpUserId = user.Id,
                    Address = input.Address,
                    ShopName = input.ShopName,
                    ShopImageurl = input.ShopImageFile != null 
                        ? await AppHelpers.InsertAndGetUrlAzure(input.ShopImageFile, "IMG_" + Guid.NewGuid(), "toko")
                        : string.Empty,
                    Handphone = input.Handphone,
                    Latitude = input.Longitude,
                    Longitude = input.Longitude,
                    Jabatan = input.Jabatan,
                    CreatorUsername = input.Username,
                    CreationTime = DateTime.UtcNow.AddHours(7),
                };

                _externalUserRepository.Insert(externalUser);

                return new ServiceResult { IsSuccess = true, Message = "Registration Success" };
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return new ServiceResult { IsSuccess = false, Message = e.Message }; 
            }

        }

        public async Task<ServiceResult> InternalUserRegister(RegisterInternalUserDto input)
        {
            try
            {

                int test = int.Parse(input.Username);
                input.Username = test.ToString();
                var internalUser = _internalUserRepository.FirstOrDefault(int.Parse(input.Username));

                if (internalUser == null)
                { return new ServiceResult { IsSuccess = false, Message = "ID MPM tidak ditemukan" }; }
                else
                {
                    if (internalUser.AbpUserId != null)
                    { return new ServiceResult { IsSuccess = false, Message = "Anda sudah registrasi sebelumnya" }; }
                }

                //var user = await _userRegistrationManager.RegisterMobileAsync(
                //    internalUser.Nama,
                //    internalUser.Nama,
                //    internalUser.Email,
                //    input.Username,
                //    input.Password,
                //    true
                //);
                string email = internalUser.Email;
                if (string.IsNullOrEmpty(email) || email == "-") 
                {
                    string[] names = internalUser.Nama.Split(" ");
                    email = names[0] + "@mpm.com";
                }

                var user = await _userRegistrationManager.RegisterAsync(
                    internalUser.Nama,
                    internalUser.Nama,
                    email,
                    input.Username,
                    input.Password,
                    true
                );

                internalUser.AbpUserId = user.Id;
                internalUser.LastModifierUsername = input.Username;
                internalUser.LastModificationTime = DateTime.UtcNow.AddHours(7);
                _internalUserRepository.Update(internalUser);

                return new ServiceResult { IsSuccess = true, Message = "Registration Success" };
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return new ServiceResult { IsSuccess = false, Message = e.Message }; 
            }
        }

        public async Task<ServiceResult> MobileResetPassword(MobileResetPasswordDto input) 
        {
            
            long? abpUserId =  null;
            try
            {
               var internalUser = _internalUserRepository.GetAll().Where(x => x.IDMPM == int.Parse(input.Username)).FirstOrDefault();
                if (internalUser != null)
                    abpUserId = internalUser.AbpUserId;
            }
            catch(Exception e) { _logger.Error(e.Message); }

            try 
            {
                var externalUser = _externalUserRepository.GetAll().FirstOrDefault(x => x.UserName == input.Username);
                if (externalUser != null)
                    abpUserId = externalUser.AbpUserId; 
            }
            catch(Exception e) { _logger.Error(e.Message); }

            if (abpUserId == null) 
            { 
                return new ServiceResult { IsSuccess = false, Message = "User tidak ditemukan" };
            }
            
            var user = await _userManager.GetUserByIdAsync(abpUserId.Value);
            if (user != null)
            {
                user.Password = _passwordHasher.HashPassword(user, input.Password);
                CurrentUnitOfWork.SaveChanges();
                return new ServiceResult { IsSuccess = true, Message = "Reset Password Berhasil" };
            }
            else 
            { return new ServiceResult { IsSuccess = false, Message = "User tidak ditemukan" }; }


        }

        public ServiceResult MobileValidateUsername(string username)
        {
            long? abpUserId = null;
            try
            {
                var internalUser = _internalUserRepository.GetAll().Where(x => x.IDMPM == int.Parse(username)).FirstOrDefault();
                if (internalUser != null)
                    abpUserId = internalUser.AbpUserId;
            }
            catch (Exception e) { _logger.Error(e.Message); }

            try
            {
                var externalUser = _externalUserRepository.GetAll().FirstOrDefault(x => x.UserName == username);
                if (externalUser != null)
                    abpUserId = externalUser.AbpUserId;
            }
            catch (Exception e) { _logger.Error(e.Message); }

            if (abpUserId == null)
            {
                return new ServiceResult { IsSuccess = false, Message = "User tidak ditemukan" };
            }
            else
            {
                return new ServiceResult { IsSuccess = true, Message = "User Valid" };
            }
        }

        [AbpAuthorize()]
        public async Task<ServiceResult> UpdateProfilePicture(IFormFile imageFile)
        {
            try
            {
                var currentUser = await GetCurrentUserAsync();
                string imageUrl =  await AppHelpers.InsertAndGetUrlAzure(imageFile, "IMG_" + Guid.NewGuid(), "profiles");

                currentUser.ImageUrl = imageUrl;
                UserDto userDto = ObjectMapper.Map<UserDto>(currentUser);
                await _userAppService.Update(userDto);

                return new ServiceResult { IsSuccess = true, Message = imageUrl };
            }
            catch (Exception e)
            { 
                return new ServiceResult { IsSuccess = false, Message = e.Message };
            }
        }

        [AbpAuthorize()]
        public TemporaryMobileDashboardDto GetTemporaryMobileDashboard(string channel)
        {
            TemporaryMobileDashboardDto dashboard = new TemporaryMobileDashboardDto();

            try
            {

                // Article
                var articleQuery = _articleRepository.GetAll().Include(x => x.ArticleAttachments)
                                                     .Where(x => x.IsPublished && string.IsNullOrEmpty(x.DeleterUsername));
                switch (channel)
                {
                    case "H1":
                        articleQuery = articleQuery.Where(x => x.H1);
                        break;
                    case "H2":
                        articleQuery = articleQuery.Where(x => x.H2);
                        break;
                    case "H3":
                        articleQuery = articleQuery.Where(x => x.H3);
                        break;
                    default:
                        break;
                }

                articleQuery = articleQuery.OrderByDescending(x => x.CreationTime);
                dashboard.Articles = articleQuery.Take(5).ToList();


                // Brand Campaign
                var brandCampaignQuery = _brandCampaignRepository.GetAll().Include(x => x.BrandCampaignAttachments)
                                                                 .Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.IsPublished)
                                                                 .OrderByDescending(x => x.CreationTime);
                try
                {
                    dashboard.BrandCampaigns = brandCampaignQuery.First();
                }
                catch (Exception e)
                {
                    _logger.Error(e.Message);
                }

                // Sales Talk
                var salesTalkQuery = _salesTalkRepository.GetAll().Include(x => x.SalesTalkAttachments)
                                                         .Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.IsPublished)
                                                         .OrderByDescending(x => x.EndDate);
                List<SalesTalks> salesTalk = new List<SalesTalks>();
                salesTalk = salesTalkQuery.Take(5).ToList();


                // Ssrvice Talk
                var serviceTalkFlyerQuery = _serviceTalkFlyerRepository.GetAll().Include(x => x.ServiceTalkFlyerAttachments)
                                                                       .Where(x => string.IsNullOrEmpty(x.DeleterUsername) && x.IsPublished)
                                                                       .OrderByDescending(x => x.EndDate);
                List<ServiceTalkFlyers> serviceTalkFlyer = new List<ServiceTalkFlyers>();
                serviceTalkFlyer = serviceTalkFlyerQuery.Take(5).ToList();

                if (channel == "H1")
                {
                    dashboard.SalesTalks = salesTalk;
                    dashboard.ServiceTalkFlyers = new List<ServiceTalkFlyers>();
                }
                else if (channel == "H2")
                {
                    dashboard.SalesTalks = new List<SalesTalks>();
                    dashboard.ServiceTalkFlyers = serviceTalkFlyer;
                }
                else if (channel == "H3")
                {
                    dashboard.SalesTalks = new List<SalesTalks>();
                    dashboard.ServiceTalkFlyers = new List<ServiceTalkFlyers>();
                }
                else if (string.IsNullOrEmpty(channel))
                {
                    dashboard.SalesTalks = salesTalk;
                    dashboard.ServiceTalkFlyers = serviceTalkFlyer;
                }
            }
            catch (Exception e) { _logger.Error(e.Message); }

            return dashboard;
        }

        [AbpAuthorize()]
        public async Task<LoginExtensionDto> GetUserProfile(string username)
        {
            User currentUser = null;
            string imageUrl = null;
            try
            {
                try
                {
                    currentUser = await GetCurrentUserAsync();
                }
                catch (Exception e) { _logger.Error(e.Message + "\n" + e.InnerException.Message); }

                if (currentUser != null)
                    imageUrl = currentUser.ImageUrl;

                var internalUser = _internalUserRepository.GetAll().FirstOrDefault(x => x.IDMPM.ToString() == username);
                if (internalUser != null)
                {
                    return new LoginExtensionDto()
                    {
                        Username = internalUser.IDMPM.ToString(),
                        Name = internalUser.Nama,
                        IDGroupJabatan = internalUser.IDGroupJabatan,
                        IDJabatan = internalUser.IDJabatan,
                        Jabatan = internalUser.Jabatan,
                        Channel = internalUser.Channel,
                        KodeDealer = internalUser.KodeDealerMPM,
                        Dealer = internalUser.DealerName,
                        Kota = internalUser.DealerKota,
                        ImageUrl = imageUrl,
                        IsAbleToRegisterCSChampionClub = IsAbleToRegister(internalUser.IDMPM)
                    };
                }
                else
                {
                    var externalUser = _externalUserRepository.GetAll().FirstOrDefault(x => x.UserName.ToLower() == username.ToLower());
                    if (externalUser != null)
                    {
                        return new LoginExtensionDto()
                        {
                            Username = externalUser.UserName,
                            Name = externalUser.Name,
                            Jabatan = externalUser.Jabatan,
                            Channel = externalUser.Channel,
                            ImageUrl = imageUrl,
                            IsAbleToRegisterCSChampionClub = false
                        };
                    }
                    else
                    {
                        return new LoginExtensionDto();
                    }
                }
            }
            catch (Exception e) 
            {
                _logger.Error(e.Message);
                return new LoginExtensionDto();
            }

        }

        public bool IsAbleToRegister(int idmpm)
        {
            try
            {
                var now = DateTime.Now;
                var participant = _csChampionClubParticipantRepository.GetAll()
                            .FirstOrDefault(x => x.IDMPM == idmpm && x.Year == now.Year);
                if (participant != null)
                    return false;

                var registration = _csChampionClubRegistrationRepository.GetAll().FirstOrDefault(x => x.Year == now.Year);
                if (registration != null)
                {
                    if (now.Date >= registration.StartDate.Date && now.Date <= registration.EndDate.Date)
                        return true;
                    else
                        return false;
                }

                return false;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return false;
            }
        }

        public async Task<ServiceResult> SendOTP(SendOTPDto input)
        {
            try
            {
                try
                {
                    if(input.UserCategory == null){
                        input.UserCategory ="";
                    }
                    if (input.UserCategory.ToLower() == "siswa")
                    {
                        var userSiswa = _tbsmSiswaRepository.FirstOrDefault(x => x.NIS == input.Username);
                        if (userSiswa != null)
                        {
                            if (!string.IsNullOrEmpty(userSiswa.NoTelp))
                                input.Handphone = userSiswa.NoTelp;
                            else
                                return new ServiceResult { IsSuccess = false, Message = "No HP tidak ditemukan" };
                        }
                    }
                    else if(input.UserCategory.ToLower() == "guru")
                    {
                        var userGuru = _tbsmGuruRepository.FirstOrDefault(x => x.NIP == input.Username);
                        if (userGuru != null)
                        {
                            if (!string.IsNullOrEmpty(userGuru.NoTelp))
                                input.Handphone = userGuru.NoTelp;
                            else
                                return new ServiceResult { IsSuccess = false, Message = "No HP tidak ditemukan" };
                        }
                    }else if(input.UserCategory.ToLower() == "h3")
                    {
                        var userH3 = _externalUserRepository.FirstOrDefault(x => x.Handphone == input.Username);
                        if (userH3 != null)
                        {
                            if (!string.IsNullOrEmpty(userH3.Handphone))
                                input.Handphone = userH3.Handphone;
                            else
                                return new ServiceResult { IsSuccess = false, Message = "No HP tidak ditemukan" };
                        }
                    }
                    else
                    {
                        var internalUser = _internalUserRepository.GetAll().Where(x => x.IDMPM == int.Parse(input.Username)).FirstOrDefault();
                        if (internalUser != null)
                        {
                            if (!string.IsNullOrEmpty(internalUser.Handphone))
                                input.Handphone = internalUser.Handphone;
                            else
                                return new ServiceResult { IsSuccess = false, Message = "No HP tidak ditemukan" };

                        }
                    }
                }
                catch { }

                var key = KeyGeneration.GenerateRandomKey(20);
                var base32String = Base32Encoding.ToString(key);
                var base32Bytes = Base32Encoding.ToBytes(base32String);
                var totp = new Totp(base32Bytes, totpSize: 4, step: 86400);
                string code = totp.ComputeTotp();
                string phone = input.Handphone;

                // OTP to Labib
                //phone = "087710105930";

                //OTP to Yohana
                //phone = "6281578430603";

                //OTP to Alby Wit
                //phone = "6285721622577";
                //phone = "6282240351018";

                //Test OTP MPM
                //phone = "082223325157";
                
                // phone = "081578430603";
                if (phone.StartsWith("0"))
                { phone = "62" + phone.Substring(1); }


                //IList<KeyValuePair<string, string>> smsAPiParameters = new List<KeyValuePair<string, string>> {
                //    { new KeyValuePair<string, string>("userkey", AppConstants.UserKey) },
                //    { new KeyValuePair<string, string>("passkey", AppConstants.PassKey) },
                //    { new KeyValuePair<string, string>("nohp", phone) },
                //    { new KeyValuePair<string, string>("pesan", CreateSMS(code)) }
                //};

                if(string.IsNullOrEmpty(phone))
                    return new ServiceResult { IsSuccess = false, Message = "No HP tidak ditemukan" };

                InfobipSms sms = new InfobipSms()
                {
                    messages = new List<InfobipMessage>()
                {
                    new InfobipMessage()
                    {
                        destinations = new List<InfobipDestination>()
                        {
                            new InfobipDestination(){ to = phone },
                        },
                        //text = "Test OTP 1"
                        text = CreateSMS(code)
                    },
                },
                };

                string json = JsonConvert.SerializeObject(sms);

                HttpClient client = new HttpClient();
                //await client.PostAsync(AppConstants.SmsApiUrl, new FormUrlEncodedContent(smsAPiParameters));
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("App", AppConstants.InfobipAPIKeyAuthorization);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", AppConstants.InfobipBase64Auhtorization);
                var result = await client.PostAsync(AppConstants.InfobipUrl, new StringContent(json, Encoding.UTF8, "application/json"));

                return new ServiceResult { IsSuccess = true, Message = base32String };
                //return base32String;
            }
            catch (Exception e) 
            {
                _logger.Error(e.Message);
                return new ServiceResult { IsSuccess = false, Message = "Gagal Mengirim OTP" };
            }

        }

        private string CreateSMS(string code)
        { return "[MPM.FLP] Your Confirmation code is " + code; }

        [AbpAuthorize()]
        public ServiceResult UpdateImageExternalUser(UpdateExternalUserImageDto input)
        {
            var externalUser = _externalUserRepository.GetAll().FirstOrDefault(x => x.UserName == input.Username);

            if (externalUser == null)
            { return new ServiceResult { IsSuccess = false, Message = "username not found" }; }

            if (!string.IsNullOrEmpty(input.UserImageUrl))
                externalUser.UserImageUrl = input.UserImageUrl;

            if (!string.IsNullOrEmpty(input.KTPImageUrl))
                externalUser.UserImageUrl = input.KTPImageUrl;

            if (!string.IsNullOrEmpty(input.ShopImageUrl))
                externalUser.UserImageUrl = input.ShopImageUrl;

            externalUser.LastModifierUsername = input.Username;
            externalUser.LastModificationTime = DateTime.UtcNow.AddHours(7);

            _externalUserRepository.Update(externalUser);

            return new ServiceResult() { IsSuccess = true, Message = "Update Success!!!" };
        }

        public List<ArticleDummy> GetListArticleDummy() 
        {
            return new List<ArticleDummy>()
            {
                new ArticleDummy(){ Title = "Article Dummy 1" },
                new ArticleDummy(){ Title = "Article Dummy 2" },
                new ArticleDummy(){ Title = "Article Dummy 3" },
                new ArticleDummy(){ Title = "Article Dummy 4" },
                new ArticleDummy(){ Title = "Article Dummy 5" },
                new ArticleDummy(){ Title = "Article Dummy 6" },
                new ArticleDummy(){ Title = "Article Dummy 7" },
                new ArticleDummy(){ Title = "Article Dummy 8" },
                new ArticleDummy(){ Title = "Article Dummy 9" },
            };
        }

        public bool IsAbleToRegisterTbsm(TbsmUserDto input)
        {
            try
            {
                #region 1. Cek NPSN sudah terdaftar atau belum
                var sekolah = _sekolahRepository.FirstOrDefault(x => x.NPSN == input.NPSN);
                if (sekolah == null)
                    return false;
                #endregion

                if (input.TBSMCategory.ToLower() == "guru")
                {
                    #region 2. Cek Nomor Induk sudah terdaftar atau belum
                    var guru = _tbsmGuruRepository.FirstOrDefault(x => x.NIP == input.NomorInduk);
                    if (guru == null)
                        return false;
                    if (guru.AbpUserId != null)
                        return false;
                    #endregion
                }
                else if (input.TBSMCategory.ToLower() == "siswa")
                {
                    #region 2. Cek Nomor Induk sudah terdaftar atau belum
                    var siswa = _tbsmSiswaRepository.FirstOrDefault(x => x.NIS == input.NomorInduk);
                    if (siswa == null)
                        return false;
                    if (siswa.AbpUserId != null)
                        return false;
                    #endregion
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return false;
            }
        }

        public async Task<ServiceResult> TbsmUserRegister(RegisterTbsmUserDto input)
        {
            try
            {
                if (input.TBSMCategory.ToLower() == "guru")
                {
                    #region 1. Cek Nomor Induk sudah terdaftar atau belum
                    var guru = _tbsmGuruRepository.FirstOrDefault(x => x.NIP == input.NomorInduk && x.NPSN == input.NPSN);
                    if (guru == null)
                        return new ServiceResult { IsSuccess = false, Message = "NIP belum terdaftar" };
                    if (guru.AbpUserId != null)
                        return new ServiceResult { IsSuccess = false, Message = "NIP sudah memiliki akun aktif" };
                    #endregion

                    #region 2. Register User Guru
                    if (string.IsNullOrEmpty(guru.Email) || guru.Email == "-")
                    {
                        string[] names = guru.Nama.Split(" ");
                        guru.Email = names[0].ToLower() + "@mpm.com";
                    }

                    var user = await _userRegistrationManager.RegisterAsync(
                        guru.Nama,
                        guru.Nama,
                        guru.Email,
                        guru.NIP,
                        input.Password,
                        true
                    );

                    guru.AbpUserId = user.Id;
                    guru.LastModifierUsername = input.NomorInduk;
                    guru.LastModificationTime = DateTime.UtcNow.AddHours(7);
                    _tbsmGuruRepository.Update(guru);
                    #endregion
                }
                else if (input.TBSMCategory.ToLower() == "siswa")
                {
                    #region 1. Cek Nomor Induk sudah terdaftar atau belum
                    var siswa = _tbsmSiswaRepository.FirstOrDefault(x => x.NIS == input.NomorInduk && x.NPSN == input.NPSN);
                    if (siswa == null)
                        return new ServiceResult { IsSuccess = false, Message = "NIS belum terdaftar" };
                    if (siswa.AbpUserId != null)
                        return new ServiceResult { IsSuccess = false, Message = "NIS sudah memiliki akun aktif" };
                    #endregion

                    #region 2. Register User Siswa
                    if (string.IsNullOrEmpty(siswa.Email) || siswa.Email == "-")
                    {
                        string[] names = siswa.Nama.Split(" ");
                        siswa.Email = names[0].ToLower() + "@mpm.com";
                    }

                    var user = await _userRegistrationManager.RegisterAsync(
                        siswa.Nama,
                        siswa.Nama,
                        siswa.Email,
                        siswa.NIS,
                        input.Password,
                        true
                    );

                    siswa.AbpUserId = user.Id;
                    siswa.LastModifierUsername = input.NomorInduk;
                    siswa.LastModificationTime = DateTime.UtcNow.AddHours(7);
                    _tbsmSiswaRepository.Update(siswa);
                    #endregion
                }
                else
                {
                    return new ServiceResult { IsSuccess = false, Message = "Kategori user salah" };
                }

                return new ServiceResult { IsSuccess = true, Message = "Registration Success" };
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return new ServiceResult { IsSuccess = false, Message = e.Message };
            }
        }
    }

    public class ArticleDummy { public string Title { get; set; } }
}
