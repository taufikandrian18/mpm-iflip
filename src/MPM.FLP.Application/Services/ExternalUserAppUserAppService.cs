using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Authorization;
using MPM.FLP.Authorization.Users;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.LogActivity;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class ExternalUserAppService : FLPAppServiceBase, IExternalUserAppService
    {
        private readonly IRepository<ExternalUsers, Guid> _externalUserRepository;
        private readonly UserManager _userManager;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;

        public ExternalUserAppService(
            IRepository<ExternalUsers, Guid> externalUserRepository,
            UserManager userManager,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService)
        {
            _externalUserRepository = externalUserRepository;
            _userManager = userManager;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public IQueryable<ExternalUsers> GetAllBackoffice()
        {
            return _externalUserRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername));
        }

        public async Task<List<ExternalUserDto>> GetAll()
        {
            var externalUsers = _externalUserRepository.GetAll().ToList();
            List<ExternalUserDto> externalUserDtos = new List<ExternalUserDto>();
            foreach (var externalUser in externalUsers)
            {
                var externalUserDto = new ExternalUserDto();
                var abpUser = await _userManager.GetUserByIdAsync(externalUser.AbpUserId);
                if (abpUser != null)
                {
                    externalUserDto.Id = externalUser.Id;
                    externalUserDto.AbpUserId = externalUser.AbpUserId;
                    externalUserDto.Channel = externalUser.Channel;
                    externalUserDto.Name = externalUser.Name;
                    externalUserDto.ShopName = externalUser.ShopName;
                    externalUserDto.ShopImageurl = externalUser.ShopImageurl;
                    externalUserDto.KTPImageUrl = externalUser.KTPImageUrl;
                    externalUserDto.UserImageUrl = externalUser.UserImageUrl;
                    externalUserDto.Address = externalUser.Address;
                    externalUserDto.Longitude = externalUser.Longitude;
                    externalUserDto.Latitude = externalUser.Latitude;
                    externalUserDto.Email = externalUser.Email;
                    externalUserDto.Handphone = externalUser.Handphone;
                    externalUserDto.Jabatan = externalUser.Jabatan;
                    externalUserDto.IsKTPVerified = externalUser.IsKTPVerified;
                    externalUserDto.IsActive = abpUser.IsActive;
                    externalUserDtos.Add(externalUserDto);
                }
            }
            return externalUserDtos;
        }

        public async Task<ExternalUserDto> GetById(Guid id) 
        {
            var externalUser = _externalUserRepository.GetAll().FirstOrDefault(x => x.Id == id);
            if (externalUser != null) 
            {
                //var externalUserDto = ObjectMapper.Map<ExternalUserDto>(externalUser);
                var externalUserDto = new ExternalUserDto();
                var abpUser = await _userManager.GetUserByIdAsync(externalUser.AbpUserId);
                if (abpUser != null)
                {
                    externalUserDto.Id = externalUser.Id;
                    externalUserDto.Channel = externalUser.Channel;
                    externalUserDto.Name = externalUser.Name;
                    externalUserDto.ShopName = externalUser.ShopName;
                    externalUserDto.ShopImageurl = externalUser.ShopImageurl;
                    externalUserDto.KTPImageUrl = externalUser.KTPImageUrl;
                    externalUserDto.UserImageUrl = externalUser.UserImageUrl;
                    externalUserDto.Address = externalUser.Address;
                    externalUserDto.Longitude = externalUser.Longitude;
                    externalUserDto.Latitude = externalUser.Latitude;
                    externalUserDto.Email = externalUser.Email;
                    externalUserDto.Handphone = externalUser.Handphone;
                    externalUserDto.Jabatan = externalUser.Jabatan;
                    externalUserDto.IsKTPVerified = externalUser.IsKTPVerified;
                    externalUserDto.IsActive = abpUser.IsActive;
                    return externalUserDto;
                }

                return null;
            }

            return null;
        }

        public async Task<ExternalUserDto> GetByAbpId(long id)
        {
            ExternalUsers externalUser = await _externalUserRepository.GetAll().Where(x => x.AbpUserId == id).FirstOrDefaultAsync();
            ExternalUserDto externalUserDto = null;
            if (externalUser != null) externalUserDto = ObjectMapper.Map<ExternalUserDto>(externalUser);
            return externalUserDto;
        }

        public async Task Update(UpdateExternalUserDto input)
        {
            var user = await _userManager.GetUserByIdAsync(input.AbpUserId);
            user.IsActive = input.IsActive;
            user.LastModificationTime = input.LastModificationTime;
            user.LastModifierUser = input.LastModifierUser;
            CheckErrors(await _userManager.UpdateAsync(user));

            var oldObject = _externalUserRepository.FirstOrDefault(x => x.Id == input.Id);
            var externalUser = _externalUserRepository.FirstOrDefault(x => x.Id == input.Id);
            externalUser.IsKTPVerified = input.IsKTPVerified;
            externalUser.LastModifierUsername = input.LastModifierUser.UserName;
            externalUser.LastModificationTime = input.LastModificationTime;
            _externalUserRepository.Update(externalUser);

            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUser.UserName, "External User", input.Id, externalUser.Name, LogAction.Update.ToString(), oldObject, externalUser);
        }

    }
}