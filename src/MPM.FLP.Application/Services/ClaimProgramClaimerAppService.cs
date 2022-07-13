using Abp.Authorization;
using Abp.Domain.Repositories;
using CorePush.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class ClaimProgramClaimerAppService : FLPAppServiceBase, IClaimProgramClaimerAppService
    {
        private readonly IRepository<ClaimProgramClaimers, Guid> _claimProgramClaimerRepository;
        private readonly IRepository<ClaimPrograms, Guid> _claimProgramRepository;
        private readonly IRepository<PushNotificationSubscribers, Guid> _pushNotificationSubscriberRepository;
        private readonly IRepository<ExternalUsers, Guid> _externalUserRepository;
        private readonly IRepository<InternalUsers> _internalUserRepository;
        private readonly IRepository<WebNotifications, Guid> _webNotificationRepository;

        public ClaimProgramClaimerAppService(IRepository<ClaimProgramClaimers, Guid> claimProgramClaimerRepository,
                                             IRepository<ClaimPrograms, Guid> claimProgramRepository,
                                             IRepository<PushNotificationSubscribers, Guid> pushNotificationSubscriberRepository,
                                             IRepository<ExternalUsers, Guid> externalUserRepository, 
                                             IRepository<InternalUsers> internalUserRepository,
                                             IRepository<WebNotifications, Guid> webNotificationRepository)
        {
            _claimProgramClaimerRepository = claimProgramClaimerRepository;
            _claimProgramRepository = claimProgramRepository;
            _pushNotificationSubscriberRepository = pushNotificationSubscriberRepository;
            _externalUserRepository = externalUserRepository;
            _internalUserRepository = internalUserRepository;
            _webNotificationRepository = webNotificationRepository;
        }

        public ClaimProgramClaimers GetById(Guid id)
        {
            var ClaimProgramClaimer = _claimProgramClaimerRepository.FirstOrDefault(x => x.Id == id);
            return ClaimProgramClaimer;
        }

        public async Task<ServiceResult> Create([FromForm] NewClaimerDto input)
        {
            try
            {
                var id = Guid.NewGuid();
                ClaimProgramClaimers claimer = new ClaimProgramClaimers()
                {
                    Id = id,
                    CreatorUsername = input.ClaimerUsername,
                    CreationTime = DateTime.UtcNow.AddHours(7),
                    ClaimerUsername = input.ClaimerUsername,
                    ClaimProgramId = input.ClaimProgramId,
                    StorageUrl = await AppHelpers.InsertAndGetUrlAzure(input.file, "IMG_" + id, "claimers")
                };
                _claimProgramClaimerRepository.Insert(claimer);

                var claimerNotification = new ClaimerWebNotificationDto()
                {
                    ClaimProgramId = claimer.ClaimProgramId,
                    ClaimProgramClaimerId = claimer.Id,
                    CreatorUsername = claimer.CreatorUsername,
                    IsReclaim = false
                };

                CreateNotification(claimerNotification);

                return new ServiceResult() { IsSuccess = true, Message = "Insert Data Success" };
            }
            catch (Exception e)
            { return new ServiceResult() { IsSuccess = false, Message = e.Message }; }
        }

        public async Task<ServiceResult> Update([FromForm] UpdateClaimerDto input) 
        {
            try
            {
                var claimer = _claimProgramClaimerRepository.GetAll().FirstOrDefault(x => x.Id == input.ClaimProgramClaimerId);
                
                if(claimer == null)
                return new ServiceResult() { IsSuccess = false, Message = "Claimer not found" };

                claimer.IsApproved = null;
                claimer.StorageUrl = await AppHelpers.InsertAndGetUrlAzure(input.file, "IMG_" + Guid.NewGuid(), "claimers");
                claimer.LastModifierUsername = claimer.CreatorUsername;
                claimer.LastModificationTime = DateTime.UtcNow.AddHours(7);

                _claimProgramClaimerRepository.Update(claimer);

                var claimerNotification = new ClaimerWebNotificationDto()
                {
                    ClaimProgramId = claimer.ClaimProgramId,
                    ClaimProgramClaimerId = claimer.Id,
                    CreatorUsername = claimer.CreatorUsername,
                    IsReclaim = true
                };

                CreateNotification(claimerNotification);

                return new ServiceResult() { IsSuccess = true, Message = "Insert Data Success" };
            }
            catch (Exception e)
            { return new ServiceResult() { IsSuccess = false, Message = e.Message }; }
        }

        async Task CreateNotification(ClaimerWebNotificationDto input) 
        {
            var claim = _claimProgramRepository.GetAll().FirstOrDefault(x => x.Id == input.ClaimProgramId);
            var url = "/ClaimPrograms/ClaimedPeople/";
            if (claim != null) 
            {
                string body = input.IsReclaim ? $"User {input.CreatorUsername} melakukan claim baru pada {claim.Title}" :
                    $"User {input.CreatorUsername} melakukan claim ulang pada {claim.Title}";
                var webNotification = new WebNotifications()
                {
                    Id = Guid.NewGuid(),
                    CreationTime = DateTime.UtcNow.AddHours(7),
                    CreatorUsername = input.CreatorUsername,
                    ReceiverUsername = claim.CreatorUsername,
                    Menu = "Claim Program",
                    ItemId = input.ClaimProgramClaimerId,
                    Header = claim.Title,
                    Body = body,
                    IsRead = false,
                    Url = url + input.ClaimProgramId
                };

                _webNotificationRepository.Insert(webNotification);

            }
        }

        public async Task<ServiceResult> TestUpload(IFormFile file) 
        {
            try
            {
                string url = await AppHelpers.InsertAndGetUrlAzure(file, "IMG_" + Guid.NewGuid(), "test");
                return new ServiceResult { IsSuccess = true, Message = url };
            }
            catch (Exception e)
            { return new ServiceResult() { IsSuccess = false, Message = e.Message }; }
        }
        public List<ClaimerDto> GetClaimers(Guid claimProgramId)
        {
            List<ClaimerDto> claimerDtos = new List<ClaimerDto>();

            var claimers = _claimProgramClaimerRepository.GetAll().Where(x => x.ClaimProgramId == claimProgramId).ToList();

            foreach (var claimer in claimers)
            {
                ClaimerDto claimerDto = new ClaimerDto();
                var externaluser = _externalUserRepository.GetAll().FirstOrDefault(x => x.UserName.ToLower() == claimer.ClaimerUsername.ToLower());
                if (externaluser != null)
                {
                    claimerDto.ClaimProgramClaimerId = claimer.Id;
                    claimerDto.ClaimerUsername = claimer.ClaimerUsername;
                    claimerDto.StorageUrl = claimer.StorageUrl;
                    claimerDto.ClaimProgramId = claimer.ClaimProgramId;
                    claimerDto.IsApproved = claimer.IsApproved;
                    claimerDto.OTP = claimer.OTP;
                    claimerDto.IsVerified = claimer.IsVerified;
                    claimerDto.VerifiedTime = claimer.VerifiedTime;
                    //claimerDto.ShopName = externaluser.ShopName;
                    //claimerDto.ShopImageurl = externaluser.ShopImageurl;
                    //claimerDto.KTPImageUrl = externaluser.KTPImageUrl;
                    //claimerDto.Address = externaluser.Address;
                    //claimerDto.Longitude = externaluser.Longitude;
                    //claimerDto.Latitude = externaluser.Latitude;
                    claimerDto.Email = externaluser.Email;
                    claimerDto.Handphone = externaluser.Handphone;
                    //claimerDto.Jabatan = externaluser.Jabatan;
                    //claimerDto.IsKTPVerified = externaluser.IsKTPVerified;
                    claimerDto.UserImageUrl = externaluser.UserImageUrl;

                    claimerDtos.Add(claimerDto);
                }
                else 
                {
                    int idflp;
                    bool result = int.TryParse(claimer.ClaimerUsername, out idflp);
                    if (result) 
                    {
                        var internalUser = _internalUserRepository.GetAll().FirstOrDefault(x => x.IDMPM == idflp);
                        if (internalUser != null) 
                        {
                            claimerDto.ClaimProgramClaimerId = claimer.Id;
                            claimerDto.ClaimerUsername = claimer.ClaimerUsername;
                            claimerDto.StorageUrl = claimer.StorageUrl;
                            claimerDto.ClaimProgramId = claimer.ClaimProgramId;
                            claimerDto.IsApproved = claimer.IsApproved;
                            claimerDto.OTP = claimer.OTP;
                            claimerDto.IsVerified = claimer.IsVerified;
                            claimerDto.VerifiedTime = claimer.VerifiedTime;
                            claimerDto.Email = internalUser.Email;
                            claimerDto.Handphone = internalUser.Handphone;
                            //claimerDto.Jabatan = internalUser.Jabatan;
                            claimerDto.KodeDealer = internalUser.KodeDealerMPM;
                            claimerDto.NamaDealer = internalUser.DealerName;
                            claimerDto.KotaDealer = internalUser.DealerKota;
                            //claimerDto.IDFLP = internalUser.IDMPM;

                            claimerDtos.Add(claimerDto);
                        }
                    }
                } 
            }

            return claimerDtos;
        }

        public async void Approve(ApprovalClaimDto input)
        {
            var claimer = _claimProgramClaimerRepository.GetAll().FirstOrDefault(x => x.Id == input.ClaimProgramClaimerId);
            if (claimer != null)
            {
                claimer.IsApproved = input.IsApproved;
                if (input.IsApproved)
                {
                    var key = KeyGeneration.GenerateRandomKey(20);
                    var base32String = Base32Encoding.ToString(key);
                    var base32Bytes = Base32Encoding.ToBytes(base32String);
                    var totp = new Totp(base32Bytes, totpSize: 6, step: 86400);
                    claimer.OTP = totp.ComputeTotp();
                }
                else
                {
                    claimer.OTP = null;
                }
                await SendClaimProgramNotification(new NotifikasiClaimerDto { 
                    ClaimProgramId = claimer.ClaimProgramId,
                    username = claimer.ClaimerUsername,
                    IsApproved = claimer.IsApproved.Value,
                });


                _claimProgramClaimerRepository.Update(claimer);
            }
        }

        public void RecreateOTP(Guid input)
        {
            var claimer = _claimProgramClaimerRepository.GetAll().FirstOrDefault(x => x.Id == input);
            if (claimer != null)
            {

                var key = KeyGeneration.GenerateRandomKey(20);
                var base32String = Base32Encoding.ToString(key);
                var base32Bytes = Base32Encoding.ToBytes(base32String);
                var totp = new Totp(base32Bytes, totpSize: 6, step: 86400);
                claimer.OTP = totp.ComputeTotp();

                _claimProgramClaimerRepository.Update(claimer);
            }
        }

        public ServiceResult Verifiy(VerifyClaimDto input)
        {
            bool isSuccess;
            string message;

            var claimer = _claimProgramClaimerRepository.GetAll().FirstOrDefault(x => x.Id == input.ClaimProgramClaimerId);
            if (claimer != null)
            {
                if (claimer.OTP == input.OTP)
                {
                    claimer.IsVerified = true;
                    claimer.VerifiedTime = DateTime.UtcNow.AddHours(7);
                    claimer.LastModifierUsername = claimer.CreatorUsername;
                    claimer.LastModificationTime = DateTime.UtcNow.AddHours(7);
                    _claimProgramClaimerRepository.Update(claimer);

                    isSuccess = true; message = "Claim Program is verified successfuly";
                }
                else
                {
                    isSuccess = false; message = "OTP is not valid";
                }
            }
            else { isSuccess = false; message = "Data not found"; }

            return new ServiceResult() { IsSuccess = isSuccess, Message = message };
        }

        public void SoftDelete(Guid id, string username)
        {
            var ClaimProgramClaimer = _claimProgramClaimerRepository.FirstOrDefault(x => x.Id == id);
            ClaimProgramClaimer.DeleterUsername = username;
            ClaimProgramClaimer.DeletionTime = DateTime.Now;
            _claimProgramClaimerRepository.Update(ClaimProgramClaimer);
        }

        async Task SendClaimProgramNotification(NotifikasiClaimerDto claimer)
        {
            List<string> deviceTokens = new List<string>();
            deviceTokens = _pushNotificationSubscriberRepository.GetAll().Where(x => x.Username == claimer.username)
                                .Select(x => x.DeviceToken).ToList();

            //deviceTokens.AddRange
            //    ((
            //        from p in _pushNotificationSubscriberRepository.GetAll()
            //        join e in _externalUserRepository.GetAll()
            //        on p.Username equals e.UserName
            //        select p.DeviceToken
            //     ).ToList());
            var claimProgram = _claimProgramRepository.GetAll().FirstOrDefault(x => x.Id == claimer.ClaimProgramId);
            string approved = "diterima";
            if(!claimer.IsApproved)
                approved = "ditolak";

            foreach (var deviceToken in deviceTokens)
            {
                using (var fcm = new FcmSender(AppConstants.ServerKey, AppConstants.SenderID))
                {
                    string notifikasi = String.Format("Claim anda pada {0} {1}", claimProgram.Title, approved);
                    var notification = AppHelpers.CreateNotification(notifikasi);
                    await fcm.SendAsync(deviceToken, notification);
                }
            }
        }
    }
}
