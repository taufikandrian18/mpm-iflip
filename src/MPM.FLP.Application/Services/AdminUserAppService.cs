using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using MPM.FLP.Authorization;
using MPM.FLP.Authorization.Roles;
using MPM.FLP.Authorization.Users;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class AdminUserAppService : FLPAppServiceBase, IAdminUserAppService
    {
        private readonly IRepository<AdminUsers, Guid> _adminUserRepository;
        private readonly IAbpSession _abpSession;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;

        public AdminUserAppService(
            IRepository<AdminUsers, Guid> adminUserRepository,
            UserManager userManager,
            RoleManager roleManager,
            IAbpSession abpSession
        )
        {
            _adminUserRepository = adminUserRepository;
            _abpSession = abpSession;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<AdminUserDto>> GetAll()
        {
            List<AdminUserDto> adminUserDtos = new List<AdminUserDto>();
            var adminUsers = _adminUserRepository.GetAll().ToList();
            foreach (var adminUser in adminUsers) 
            {
                var abpUserId = adminUser.AbpUserId;
                var abpUser = await _userManager.GetUserByIdAsync(abpUserId);
                if (abpUser != null) 
                {
                    var adminUserDto = new AdminUserDto();
                    adminUserDto.Username = abpUser.UserName;
                    var roles = _roleManager.Roles.Where(r => abpUser.Roles.Any(ur => ur.RoleId == r.Id))
                                                  .Select(r => r.NormalizedName);
                    adminUserDto.RoleNames = roles.ToArray();
                    adminUserDto.Channel = adminUser.Channel;
                    adminUserDto.IsActive = abpUser.IsActive;
                    adminUserDtos.Add(adminUserDto);
                }
            }

            return adminUserDtos;
        }

        public async Task<AdminUserDto> GetById(Guid id) 
        {
            var adminUser = _adminUserRepository.GetAll().FirstOrDefault(x => x.Id == id);
            if (adminUser != null) 
            {
                var abpUserId = adminUser.AbpUserId;
                var abpUser = await _userManager.GetUserByIdAsync(abpUserId);
                if (abpUser != null)
                {
                    var adminUserDto = new AdminUserDto();
                    adminUserDto.Username = abpUser.UserName;
                    var roles = _roleManager.Roles.Where(r => abpUser.Roles.Any(ur => ur.RoleId == r.Id))
                                                  .Select(r => r.NormalizedName);
                    adminUserDto.RoleNames = roles.ToArray();
                    adminUserDto.Channel = adminUser.Channel;
                    adminUserDto.IsActive = abpUser.IsActive;
                    return adminUserDto;
                }
            }

            return null;
        }

        public async Task Create(CreateAdminUserDto input)
        {
            var user = ObjectMapper.Map<User>(input);

            user.TenantId = AbpSession.TenantId;
            user.IsEmailConfirmed = true;

            await _userManager.InitializeOptionsAsync(AbpSession.TenantId);

            CheckErrors(await _userManager.CreateAsync(user, input.Password));
            

            if (input.RoleNames != null)
            {
                CheckErrors(await _userManager.SetRoles(user, input.RoleNames));
            }

            long userId = _abpSession.UserId.Value;
            User loginUser = await _userManager.GetUserByIdAsync(userId);


            AdminUsers adminUsers = new AdminUsers() 
            { 
                AbpUserId = user.Id, 
                Channel = input.Channel,
                CreatorUsername = loginUser.UserName,
                CreationTime = DateTime.Now
            };
            _adminUserRepository.Insert(adminUsers);

            CurrentUnitOfWork.SaveChanges();
        }

        public async Task Update(AdminUserDto input)
        {
            var user = await _userManager.GetUserByIdAsync(input.AbpUserId);

            MapToEntity(input, user);

            CheckErrors(await _userManager.UpdateAsync(user));

            if (input.RoleNames != null)
            {
                CheckErrors(await _userManager.SetRoles(user, input.RoleNames));
            }
        }

        public string GetChannel()
        {
            if (_abpSession.UserId == null)
            {
                throw new UserFriendlyException("Please log in before attemping to change password.");
            }
            long userId = _abpSession.UserId.Value;

            return _adminUserRepository.GetAll().FirstOrDefault(x => x.AbpUserId == userId).Channel;
        }

        protected void MapToEntity(AdminUserDto input, User user)
        {
            ObjectMapper.Map(input, user);
            user.SetNormalizedNames();
        }

    }
}
