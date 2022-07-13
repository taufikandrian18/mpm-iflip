using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Authorization;
using MPM.FLP.Authorization.Users;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.LogActivity;
using MPM.FLP.MultiTenancy;
using MPM.FLP.Repositories;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public class InternalUserAppService : FLPAppServiceBase, IInternalUserAppService
    {
        private readonly IRepository<InternalUsers> _internalUserRepository;
        private readonly UserManager _userManager;
        private readonly IRepository<Dealers, string> _dealerRepository;
        private readonly IActivityLogRepository _activityLogRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;

        public InternalUserAppService(IRepository<InternalUsers> internalUserRepository, 
                                      UserManager userManager,
                                      IRepository<Dealers, string> dealerRepository,
                                      IActivityLogRepository activityLogRepository)
        {
            _internalUserRepository = internalUserRepository;
            _userManager = userManager;
            _dealerRepository = dealerRepository;
            _activityLogRepository = activityLogRepository;
        }


        public IQueryable<InternalUsers> GetAll() 
        {
            return _internalUserRepository.GetAll();
        }
        [AbpAuthorize()]

        public async Task<List<InternalUserDto>> GetAllInternalUsers(string channel)
        {
            try
            {
                var internalUsers = _internalUserRepository.GetAll()
                    .Where(x => x.Channel == channel)
                    .ToList();

                List<InternalUserDto> internalUserDtos = new List<InternalUserDto>();
                foreach (var internalUser in internalUsers)
                {
                    //var internalUserDto = ObjectMapper.Map<InternalUserDto>(internalUser);
                    var internalUserDto = new InternalUserDto();
                    if (internalUser.AbpUserId != null)
                    {
                        var abpUser = await _userManager.GetUserByIdAsync(internalUser.AbpUserId.Value);
                        if (abpUser != null)
                        {
                            internalUserDto.IDMPM = internalUser.IDMPM;
                            internalUserDto.AbpUserId = internalUser.AbpUserId;
                            internalUserDto.IDHonda = internalUser.IDHonda;
                            internalUserDto.Nama = internalUser.Nama;
                            internalUserDto.NoKTP = internalUser.NoKTP;
                            internalUserDto.Alamat = internalUser.Alamat;
                            internalUserDto.Channel = internalUser.Channel;
                            internalUserDto.Handphone = internalUser.Handphone;
                            internalUserDto.Gender = internalUser.Gender;
                            internalUserDto.AkunInstagram = internalUser.AkunInstagram;
                            internalUserDto.AkunFacebook = internalUser.AkunFacebook;
                            internalUserDto.AkunTwitter = internalUser.AkunTwitter;

                            internalUserDto.Jabatan = internalUser.Jabatan;
                            internalUserDto.NamaAtasan = internalUser.NamaAtasan;
                            internalUserDto.DealerName = internalUser.DealerName;
                            internalUserDto.DealerKota = internalUser.DealerKota;
                            internalUserDto.IsActive = abpUser.IsActive;
                            internalUserDto.KodeDealerMPM = internalUser.KodeDealerMPM;
                            internalUserDtos.Add(internalUserDto);
                        }
                    }
                }
                return internalUserDtos;
            }
            catch (Exception e) 
            { List<InternalUserDto> internalUserDtos = new List<InternalUserDto>(); return internalUserDtos; }
           
        }

        public async Task<InternalUserDto> GetById(int idMPM) 
        {
            var internalUser = _internalUserRepository.GetAll().FirstOrDefault(x => x.IDMPM == idMPM);
            if (internalUser != null) 
            {
                //var internalUserDto = ObjectMapper.Map<InternalUserDto>(internalUser);
                var internalUserDto = new InternalUserDto();

                if (internalUser.AbpUserId != null)
                {
                    var abpUser = await _userManager.GetUserByIdAsync(internalUser.AbpUserId.Value);
                    if (abpUser != null)
                    {
                        internalUserDto.IDMPM = internalUser.IDMPM;
                        internalUserDto.AbpUserId = internalUser.AbpUserId;
                        internalUserDto.IDHonda = internalUser.IDHonda;
                        internalUserDto.Nama = internalUser.Nama;
                        internalUserDto.NoKTP = internalUser.NoKTP;
                        internalUserDto.Alamat = internalUser.Alamat;
                        internalUserDto.Channel = internalUser.Channel;
                        internalUserDto.Handphone = internalUser.Handphone;
                        internalUserDto.Gender = internalUser.Gender;
                        internalUserDto.AkunInstagram = internalUser.AkunInstagram;
                        internalUserDto.AkunFacebook = internalUser.AkunFacebook;
                        internalUserDto.AkunTwitter = internalUser.AkunTwitter;

                        internalUserDto.Jabatan = internalUser.Jabatan;
                        internalUserDto.NamaAtasan = internalUser.NamaAtasan;
                        internalUserDto.DealerName = internalUser.DealerName;
                        internalUserDto.DealerKota = internalUser.DealerKota;
                        internalUserDto.IsActive = abpUser.IsActive;
                        return internalUserDto;
                    }
                }
                else 
                {
                    internalUserDto.IDMPM = internalUser.IDMPM;
                    internalUserDto.AbpUserId = internalUser.AbpUserId;
                    internalUserDto.IDHonda = internalUser.IDHonda;
                    internalUserDto.Nama = internalUser.Nama;
                    internalUserDto.NoKTP = internalUser.NoKTP;
                    internalUserDto.Alamat = internalUser.Alamat;
                    internalUserDto.Channel = internalUser.Channel;
                    internalUserDto.Handphone = internalUser.Handphone;
                    internalUserDto.Gender = internalUser.Gender;
                    internalUserDto.AkunInstagram = internalUser.AkunInstagram;
                    internalUserDto.AkunFacebook = internalUser.AkunFacebook;
                    internalUserDto.AkunTwitter = internalUser.AkunTwitter;

                    internalUserDto.Jabatan = internalUser.Jabatan;
                    internalUserDto.NamaAtasan = internalUser.NamaAtasan;
                    internalUserDto.DealerName = internalUser.DealerName;
                    internalUserDto.DealerKota = internalUser.DealerKota;
                    internalUserDto.IsActive = false;
                    return internalUserDto;
                }

                return null;
            }

            return null;
        }

        public List<InternalUsers> GetDirectSubordinates(int idmpm)
        {
            return _internalUserRepository.GetAll().Where(x => x.MPMKodeAtasan.Value == idmpm).ToList();
        }

        public List<InternalUsers> GetCOESubordinates(int idmpm)
        {
            var user = _internalUserRepository.GetAll().FirstOrDefault(x => x.IDMPM == idmpm);
            if (user != null) 
            {
                if(user.Jabatan == "KEPALA CABANG")
                {
                    return _internalUserRepository.GetAll().Where(x => x.KodeDealerMPM == user.KodeDealerMPM && x.IDMPM != idmpm).ToList();
                }
            }
            return _internalUserRepository.GetAll().Where(x => x.MPMKodeAtasan.Value == idmpm).ToList();
        }

        public IQueryable<InternalUsers> GetInternalUserByChannel(string channel)
        {
            return _internalUserRepository.GetAll().Where(x => x.Channel == channel);
        }

        public async Task<List<ActivitiesInternalUsersDto>> GetInternalUserByChannelIncludeActivity(string channel)
        {

            var data = await _internalUserRepository.GetAll().Where(x => x.Channel == channel).Select(iUser =>
                         new ActivitiesInternalUsersDto
                         {
                             IDMPM = iUser.IDMPM.ToString(),
                             KodeDealer = iUser.KodeDealerMPM,
                             Kota = iUser.DealerKota,
                             NamaDealer = iUser.DealerName,
                             Name = iUser.Nama,
                             //Activity = _activityLogRepository.GetAll().Where(x => x.Username == iUser.IDMPM.ToString()).ToList()
                         }).ToListAsync();
            return data;
        }

        public IQueryable<InternalUsers> GetInternalUserByDealer(string channel, string kodeDealerMPM)
        {
            return _internalUserRepository.GetAll().Where(x => x.Channel == channel && x.KodeDealerMPM == kodeDealerMPM);
        }

        public List<InternalUsers> GetInternalUserByDealerMobile(string channel, string kodeDealerMPM)
        {
            return _internalUserRepository.GetAll().Where(x => x.Channel == channel && x.KodeDealerMPM == kodeDealerMPM).ToList();
        }

        public IQueryable<InternalUsers> GetInternalUserByKaresidenanH1(string karesidenan)
        {
            List <string> kodeDealers = _dealerRepository.GetAll().Where(x => x.Channel == "H1" && x.Karesidenan == karesidenan).Select(y => y.KodeDealerMPM).ToList();
            return _internalUserRepository.GetAll().Where(x => x.Channel == "H1" && kodeDealers.Contains(x.KodeDealerMPM));
        }

        public IQueryable<InternalUsers> GetInternalUserByKaresidenanHC3(string channel, string karesidenan)
        {
            List<string> kodeDealers = _dealerRepository.GetAll().Where(x => x.Channel == channel && x.NamaKaresidenanHC3 == karesidenan).Select(y => y.KodeDealerMPM).ToList();
            return _internalUserRepository.GetAll().Where(x => x.Channel == channel && kodeDealers.Contains(x.KodeDealerMPM));
        }

        public IQueryable<InternalUsers> GetInternalUserByKota(string channel, string kota)
        {
            return _internalUserRepository.GetAll().Where(x => x.Channel == channel && x.DealerKota == kota);
        }

        public async Task Update(UpdateInternalUserDto input)
        {
            var oldObject = await _userManager.GetUserByIdAsync(input.AbpUserId);
            var user = await _userManager.GetUserByIdAsync(input.AbpUserId);
            user.IsActive = input.IsActive;
            user.LastModificationTime = input.LastModificationTime;
            user.LastModifierUser = input.LastModifierUser;
            CheckErrors(await _userManager.UpdateAsync(user));
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUser.UserName, "Internal User", Guid.Empty, user.Name, LogAction.Update.ToString(), oldObject, user);
        }

    }
}
