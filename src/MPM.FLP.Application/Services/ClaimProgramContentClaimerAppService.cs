using Abp.Authorization;
using Abp.Domain.Repositories;
using CorePush.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.Authorization.Users;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
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
    public class ClaimProgramContentClaimerAppService : FLPAppServiceBase, IClaimProgramContentClaimerAppService
    {
        private readonly IRepository<ClaimProgramContentClaimers, Guid> _claimProgramClaimerRepository;
        private readonly IRepository<ClaimProgramContents, Guid> _claimProgramRepository;
        private readonly IRepository<WebNotifications, Guid> _webNotificationRepository;
        private readonly IRepository<ExternalUsers, Guid> _externalUserRepository;
        private readonly IRepository<InternalUsers> _internalUserRepository;
        private readonly IRepository<PushNotificationSubscribers, Guid> _pushNotificationSubscriberRepository;
        private readonly IRepository<ClaimProgramCampaignPrizes, Guid> _repositoryPrizes;
        private readonly IRepository<ClaimProgramCampaignPoints, Guid> _repositoryPoint;
        private readonly IRepository<ClaimProgramCampaignPrizeLogs, Guid> _repositoryPrizeLogs;
        private readonly IRepository<AdminUsers, Guid> _adminUserRepository;
        private readonly IRepository<User, long> _userRepository;
        public ClaimProgramContentClaimerAppService(IRepository<ClaimProgramContentClaimers, Guid> claimProgramClaimerRepository,
                                                    IRepository<ClaimProgramContents, Guid> claimProgramRepository,
                                                    IRepository<WebNotifications, Guid> webNotificationRepository,
                                                    IRepository<ExternalUsers, Guid> externalUserRepository,
                                                    IRepository<InternalUsers> internalUserRepository,
                                                    IRepository<PushNotificationSubscribers, Guid> pushNotificationSubscriberRepository,
                                                    IRepository<ClaimProgramCampaignPrizes, Guid> repositoryPrizes,
                                                    IRepository<ClaimProgramCampaignPoints, Guid> repositoryPoint,
                                                    IRepository<ClaimProgramCampaignPrizeLogs, Guid> repositoryPrizeLogs,
                                                    IRepository<User, long> userRepository)
        {
            _claimProgramClaimerRepository = claimProgramClaimerRepository;
            _claimProgramRepository = claimProgramRepository;
            _webNotificationRepository = webNotificationRepository;
            _externalUserRepository = externalUserRepository;
            _internalUserRepository = internalUserRepository;
            _pushNotificationSubscriberRepository = pushNotificationSubscriberRepository;
            _repositoryPrizes = repositoryPrizes;
            _repositoryPoint = repositoryPoint;
            _repositoryPrizeLogs = repositoryPrizeLogs;
            _userRepository = userRepository;
        }

        public ClaimProgramContentClaimers GetById(Guid Id)
        {
            var data = _claimProgramClaimerRepository.FirstOrDefault(x => x.Id == Id);
            return data;
        }

        public BaseResponse GetAll([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            // internal user
            var queryInternal = (from claimer in _claimProgramClaimerRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                         join claim in _claimProgramRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                         on claimer.GUIDClaimProgramContent equals claim.Id
                         join u in _userRepository.GetAll().Where(x => x.DeletionTime == null)
                         on claimer.EmployeeId equals u.Id
                         join intern in _internalUserRepository.GetAll().Where(x => x.DeletionTime == null)
                         on u.Id equals intern.AbpUserId
                         where claim.IsH1 == true orderby claim.IsH2 == true
                         select new ClaimerDto
                           {
                               ClaimProgramClaimerId = claimer.Id,
                               ClaimerUsername = u.UserName,
                               NamaUser = intern.Nama,
                               StorageUrl = claimer.StorageUrl,
                               ClaimProgramId = claim.Id,
                               IsApproved = claimer.IsApprove,
                               EnumStatus = claimer.EnumStatus,
                               OTP = claimer.OTP,
                               IsVerified = claimer.IsVerified,
                               VerifiedTime = claimer.VerifiedTime,
                               ShopName = "",
                               ShopImageurl = "",
                               KTPImageUrl = "",
                               Address = intern.Alamat,
                               //Longitude = u.Longitude,
                               //Latitude = u.Latitude,
                               Email = intern.Email,
                               Handphone = intern.Handphone,
                               Jabatan = intern.Jabatan,
                               KodeDealer = intern.KodeDealerMPM,
                               NamaDealer = intern.DealerName,
                             KotaDealer = intern.DealerKota,
                             UserImageUrl = intern.NamaFileFoto,
                               StartDate = claim.StartDate,
                               EndDate = claim.EndDate
                           }).ToList();

            var queryExternal = (from claimer in _claimProgramClaimerRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                            join claim2 in _claimProgramRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                            on claimer.GUIDClaimProgramContent equals claim2.Id
                            join u2 in _userRepository.GetAll().Where(x => x.DeletionTime == null)
                            on claimer.EmployeeId equals u2.Id
                            join external2 in _externalUserRepository.GetAll().Where(x => x.DeletionTime == null)
                            on u2.Id equals external2.AbpUserId
                            where claim2.IsH1 == true
                            orderby claim2.IsH2 == true
                         select new ClaimerDto
                         {
                             ClaimProgramClaimerId = claimer.Id,
                             ClaimerUsername = u2.UserName,
                             NamaUser = external2.Name,
                             StorageUrl = claimer.StorageUrl,
                             ClaimProgramId = claim2.Id,
                             IsApproved = claimer.IsApprove,
                             EnumStatus = claimer.EnumStatus,
                             OTP = claimer.OTP,
                             IsVerified = claimer.IsVerified,
                             VerifiedTime = claimer.VerifiedTime,
                             ShopName = external2.ShopName,
                             ShopImageurl = external2.ShopImageurl,
                             KTPImageUrl = external2.KTPImageUrl,
                             Address = external2.Address,
                             //Longitude = u.Longitude,
                             //Latitude = u.Latitude,
                             Email = external2.Email,
                             Handphone = external2.Handphone,
                             Jabatan = external2.Jabatan,
                             KodeDealer = external2.DealerCodeH3,
                             NamaDealer = external2.CategoryH3,
                             KotaDealer = external2.Kota,
                             UserImageUrl = u2.ImageUrl,
                             StartDate = claim2.StartDate,
                             EndDate = claim2.EndDate
                         }).ToList();

            var query = queryInternal.Union(queryExternal).ToList();

            if (!string.IsNullOrEmpty(request.ParentId))
            {
                query = query.Where(x => x.ClaimProgramId == Guid.Parse(request.ParentId)).ToList();
            }
            if (!string.IsNullOrEmpty(request.StartDate.ToString()) && !string.IsNullOrEmpty(request.EndDate.ToString()))
            {
                query = query.Where(x => x.StartDate >= request.StartDate && x.EndDate <= request.EndDate).ToList();
            }
            if (!string.IsNullOrEmpty(request.EnumStatus))
            {
                query = query.Where(x => x.EnumStatus == int.Parse(request.EnumStatus)).ToList();
            }

            var count = query.Count();
            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        public void Create(ClaimProgramContentClaimersCreateDto input)
        {
            var claimer = ObjectMapper.Map<ClaimProgramContentClaimers>(input);
            claimer.EmployeeId = Convert.ToInt32(this.AbpSession.UserId);
            claimer.GUIDClaimProgramPrize = Guid.Empty;
            claimer.GUIDClaimProgramPoint = Guid.Empty;
            claimer.EnumStatus = (int)ClaimProgramClaimerStatus.Pending;
            claimer.CreationTime = DateTime.Now;
            claimer.CreatorUsername = this.AbpSession.UserId.ToString();

            var claimers = _claimProgramClaimerRepository.InsertAndGetId(claimer);

            var claimerNotification = new ClaimerWebNotificationDto()
            {
                ClaimProgramId = claimer.GUIDClaimProgramContent,
                ClaimProgramClaimerId = claimer.Id,
                CreatorUsername = claimer.CreatorUsername,
                IsReclaim = false
            };
            //var task = Task.Run(() => CreateNotification(claimerNotification));
        }

        public void Update(ClaimProgramContentClaimersUpdateDto input)
        {
            var claimer = _claimProgramClaimerRepository.Get(input.Id);
            claimer.GUIDClaimProgramPrize = input.GUIDClaimProgramPrize;
            claimer.GUIDClaimProgramContent = input.GUIDClaimProgramContent;
            claimer.EmployeeId = input.EmployeeId;
            claimer.GUIDClaimProgramPoint = input.GUIDClaimProgramPoint;
            claimer.EnumStatus = input.EnumStatus;
            
            _claimProgramClaimerRepository.Update(claimer);

            var claimerNotification = new ClaimerWebNotificationDto()
            {
                ClaimProgramId = claimer.GUIDClaimProgramContent,
                ClaimProgramClaimerId = claimer.Id,
                CreatorUsername = claimer.CreatorUsername,
                IsReclaim = true
            };
            var task = Task.Run(() => CreateNotification(claimerNotification));
        }

        async Task CreateNotification(ClaimerWebNotificationDto input)
        {
            var claim = _claimProgramRepository.GetAll().FirstOrDefault(x => x.Id == input.ClaimProgramId);
            var url = "/ClaimPrograms/ClaimedPeople/";
            if (claim != null)
            {
                string body = input.IsReclaim ? $"User {input.CreatorUsername} melakukan claim baru pada {claim.Name}" :
                    $"User {input.CreatorUsername} melakukan claim ulang pada {claim.Name}";
                var webNotification = new WebNotifications()
                {
                    Id = Guid.NewGuid(),
                    CreationTime = DateTime.UtcNow.AddHours(7),
                    CreatorUsername = input.CreatorUsername,
                    ReceiverUsername = claim.CreatorUsername,
                    Menu = "Claim Program",
                    ItemId = input.ClaimProgramClaimerId,
                    Header = claim.Name,
                    Body = body,
                    IsRead = false,
                    Url = url + input.ClaimProgramId
                };

                _webNotificationRepository.Insert(webNotification);

            }
        }
        public void SoftDelete(ClaimProgramContentClaimersDeleteDto input)
        {
            var claimer = _claimProgramClaimerRepository.Get(input.Id);
            claimer.DeleterUsername = this.AbpSession.UserId.ToString();
            claimer.DeletionTime = DateTime.Now;
            _claimProgramClaimerRepository.Update(claimer);
        }
        public void Approve(ApprovalContentClaimerDto input)
        {
            var claimer = _claimProgramClaimerRepository.GetAll().FirstOrDefault(x => x.Id == input.ClaimProgramContentClaimerId);

            if (claimer != null)
            {
                claimer.IsApprove = input.IsApprove;
                if (input.IsApprove == true)
                {
                    // get claim content
                    var claim = _claimProgramRepository.FirstOrDefault(x => x.Id == claimer.GUIDClaimProgramContent);
                    if (claim.IsOtp == true)
                    {
                        var key = KeyGeneration.GenerateRandomKey(20);
                        var base32String = Base32Encoding.ToString(key);
                        var base32Bytes = Base32Encoding.ToBytes(base32String);
                        var totp = new Totp(base32Bytes, totpSize: 6, step: 86400);
                        claimer.OTP = totp.ComputeTotp();
                        claimer.EnumStatus = (int)ClaimProgramClaimerStatus.Approve;
                    }
                    else
                    {
                        claimer.EnumStatus = (int)ClaimProgramClaimerStatus.Success;
                        claimer.IsVerified = true;
                        claimer.VerifiedTime = DateTime.UtcNow.AddHours(7);
                        claimer.LastModifierUsername = claimer.CreatorUsername;
                        claimer.LastModificationTime = DateTime.UtcNow.AddHours(7);

                        // get point user
                        var dataPoint = _repositoryPoint.FirstOrDefault(x => x.GUIDClaimProgramCampaign == claim.GUIDClaimProgramCampaign && x.EmployeeId == claimer.EmployeeId);
                        dataPoint.Point = dataPoint.Point + claim.Point;
                        _repositoryPoint.Update(dataPoint);
                    }
                }
                else
                {
                    claimer.OTP = null;
                    claimer.EnumStatus = (int)ClaimProgramClaimerStatus.Reject;
                }

                var claimerNotification = new NotifikasiClaimerDto()
                {
                    ClaimProgramId = claimer.Id,
                    username = claimer.CreatorUsername,
                    IsApproved = claimer.IsApprove
                };
                //var task = Task.Run(() => SendClaimProgramNotification(claimerNotification));

                _claimProgramClaimerRepository.Update(claimer);

            }
        }

        async Task SendClaimProgramNotification(NotifikasiClaimerDto claimer)
        {
            List<string> deviceTokens = new List<string>();
            deviceTokens = _pushNotificationSubscriberRepository.GetAll().Where(x => x.Username == claimer.username)
                                .Select(x => x.DeviceToken).ToList();

            var claimProgram = _claimProgramRepository.GetAll().FirstOrDefault(x => x.Id == claimer.ClaimProgramId);
            string approved = "diterima";
            if (!claimer.IsApproved)
                approved = "ditolak";

            foreach (var deviceToken in deviceTokens)
            {
                using (var fcm = new FcmSender(AppConstants.ServerKey, AppConstants.SenderID))
                {
                    string notifikasi = String.Format("Claim anda pada {0} {1}", claimProgram.Name, approved);
                    var notification = AppHelpers.CreateNotification(notifikasi);
                    await fcm.SendAsync(deviceToken, notification);
                }
            }
        }
        public BaseResponse Verify(VerifyClaimerDto input)
        {
            bool isSuccess;
            string message;

            var claimer = _claimProgramClaimerRepository.GetAll().FirstOrDefault(x => x.Id == input.ClaimProgramContentClaimerId);
            if (claimer != null)
            {
                if (claimer.OTP == input.OTP)
                {
                    claimer.IsVerified = true;
                    claimer.VerifiedTime = DateTime.UtcNow.AddHours(7);
                    claimer.LastModifierUsername = claimer.CreatorUsername;
                    claimer.LastModificationTime = DateTime.UtcNow.AddHours(7);
                    claimer.EnumStatus = (int)ClaimProgramClaimerStatus.Success;
                    _claimProgramClaimerRepository.Update(claimer);

                    // get point user
                    var claim = _claimProgramRepository.FirstOrDefault(x => x.Id == claimer.GUIDClaimProgramContent);
                    var dataPoint = _repositoryPoint.FirstOrDefault(x => x.GUIDClaimProgramCampaign == claim.GUIDClaimProgramCampaign && x.EmployeeId == claimer.EmployeeId);
                    dataPoint.Point = dataPoint.Point + claim.Point;
                    _repositoryPoint.Update(dataPoint);

                    isSuccess = true; message = "Claim Program is verified successfuly";
                }
                else
                {
                    isSuccess = false; message = "OTP is not valid";
                }
            }
            else { isSuccess = false; message = "Data not found"; }

            return BaseResponse.Ok(message, 1);

        }

        public List<ClaimerDto> ExportExcel(FilterGetClaimerDto request)
        {
            var query = (from claimer in _claimProgramClaimerRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                         join claim in _claimProgramRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                         on claimer.GUIDClaimProgramContent equals claim.Id
                         join u in _userRepository.GetAll().Where(x => x.DeletionTime == null)
                         on claimer.EmployeeId equals u.Id
                         //join external in _externalUserRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                         //on claimer.EmployeeId equals external.AbpUserId
                         select new ClaimerDto
                         {
                             ClaimProgramClaimerId = claimer.Id,
                             ClaimerUsername = u.UserName,
                             StorageUrl = claimer.StorageUrl,
                             ClaimProgramId = claim.Id,
                             IsApproved = claimer.IsApprove,
                             EnumStatus = claimer.EnumStatus,
                             OTP = claimer.OTP,
                             IsVerified = claimer.IsVerified,
                             VerifiedTime = claimer.VerifiedTime,
                             //ShopName = external.ShopName,
                             //ShopImageurl = external.ShopImageurl,
                             //KTPImageUrl = external.KTPImageUrl,
                             //Address = external.Address,
                             //Longitude = external.Longitude,
                             //Latitude = external.Latitude,
                             Email = u.EmailAddress,
                             Handphone = u.PhoneNumber,
                             //Jabatan = external.Jabatan,
                             //IsKTPVerified = external.IsKTPVerified,
                             UserImageUrl = u.ImageUrl,
                             StartDate = claim.StartDate,
                             EndDate = claim.EndDate
                         }).ToList();

            if ( request.ClaimProgramContentId != Guid.Empty)
            {
                query = query.Where(x => x.ClaimProgramId == request.ClaimProgramContentId).ToList();
            }
            if (!string.IsNullOrEmpty(request.StartDate.ToString()) && !string.IsNullOrEmpty(request.EndDate.ToString()))
            {
                query = query.Where(x => x.StartDate >= request.StartDate && x.EndDate <= request.EndDate).ToList();
            }

            var data = query.ToList();

            return data;
        }

        public void RedeemPrize(RedeemPrizeClaimerDto input)
        {
            var currentUser = this.AbpSession.UserId;
            var prize = _repositoryPrizes.GetAll().FirstOrDefault(x => x.Id == input.ClaimProgramCampaignPrizesId);
            var pointUser = _repositoryPoint.GetAll().FirstOrDefault(x => x.GUIDClaimProgramCampaign == prize.GUIDClaimProgramCampaign && x.EmployeeId == currentUser);
            
            if (pointUser.Point >= prize.RedeemPoint)
            {
                pointUser.Point = (pointUser.Point) - (prize.RedeemPoint);
                _repositoryPoint.Update(pointUser);

                // insert log
                var logs = new ClaimProgramCampaignPrizeLogs
                {
                    GUIDClaimProgramCampaignPoints = pointUser.Id,
                    GUIDClaimProgramCampaignPrizes = input.ClaimProgramCampaignPrizesId,
                    CreationTime = DateTime.Now,
                    CreatorUsername = this.AbpSession.UserId.ToString()
                };
                _repositoryPrizeLogs.InsertAndGetId(logs);
            }
        }

        public void ResendOTP(Guid Id)
        {
            var claimer = _claimProgramClaimerRepository.GetAll().FirstOrDefault(x => x.Id == Id);
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
        public BaseResponse GetClaimByCurrentUser([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = (from claimer in _claimProgramClaimerRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                         join claim in _claimProgramRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                         on claimer.GUIDClaimProgramContent equals claim.Id
                         join point in _repositoryPoint.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername))
                         on claim.GUIDClaimProgramCampaign equals point.GUIDClaimProgramCampaign
                         select new ListClaimerDto
                         {
                             ClaimProgramClaimerId = claimer.Id,
                             IsApproved = claimer.IsApprove,
                             EnumStatus = claimer.EnumStatus,
                             ClaimProgramContentName = claim.Name,
                             StartDate = claim.StartDate,
                             EndDate = claim.EndDate,
                             Point = point.Point,
                             EmployeeId = claimer.EmployeeId,
                             StorageUrl = claimer.StorageUrl,
                             IsH1 = claim.IsH1,
                             IsH2 = claim.IsH2,
                             IsH3 = claim.IsH3,
                             IsTbsm = claim.IsTBSM
                         });

            var currentUser = this.AbpSession.UserId;
            query = query.Where(x => x.EmployeeId == currentUser);

            if (!string.IsNullOrEmpty(request.EnumStatus))
            {
                query = query.Where(x => x.EnumStatus == int.Parse(request.EnumStatus));
            }
            if (request.IsH1.HasValue)
            {
                query = query.Where(x => x.IsH1 == request.IsH1);
            }

            if (request.IsH2.HasValue)
            {
                query = query.Where(x => x.IsH2 == request.IsH2);
            }

            if (request.IsH3.HasValue)
            {
                query = query.Where(x => x.IsH3 == request.IsH3);
            }
            if (request.IsTBSM.HasValue)
            {
                query = query.Where(x => x.IsTbsm == request.IsTBSM);
            }
            var count = query.ToList().Count();
            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }
        public int GetPointByCurrentUser(Guid campaignId)
        {
            var currentUser = this.AbpSession.UserId;
            var data = _repositoryPoint.FirstOrDefault(x => x.GUIDClaimProgramCampaign == campaignId && x.EmployeeId == currentUser);
            if (data == null)
                return 0;
            else
                return data.Point;
        }
    }
}
