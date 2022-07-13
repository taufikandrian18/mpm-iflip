using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Runtime.Validation;
using MPM.FLP.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MPM.FLP.Services.Dto
{
    public class AdminUserDto
    {
        public Guid Id { get; set; }
        public long AbpUserId { get; set; }
        public string Channel { get; set; }
        public string Username { get; set; }
        public string[] RoleNames { get; set; }
        public bool IsActive { get; set; }

    }

    [AutoMapTo(typeof(User))]
    public class CreateAdminUserDto : IShouldNormalize
    {
        [Required]
        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxSurnameLength)]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        public bool IsActive { get; set; }

        public string[] RoleNames { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }

        public void Normalize()
        {
            if (RoleNames == null)
            {
                RoleNames = new string[0];
            }
        }

        [Required]
        public string Channel { get; set; }
    }

    public class MobileResetPasswordDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
