using Abp.AutoMapper;
using Microsoft.AspNetCore.Http;
using MPM.FLP.Authorization.Users;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MPM.FLP.Services.Dto
{
    [AutoMap(typeof(ExternalUsers))]
    public class ExternalUserDto
    {
        public Guid Id { get; set; }
        public long AbpUserId { get; set; }
        public string Channel { get; set; }
        public string Name { get; set; }
        public string ShopName { get; set; }
        public string ShopImageurl { get; set; }
        public string KTPImageUrl { get; set; }
        public string Address { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Email { get; set; }
        public string Handphone { get; set; }
        public string Jabatan { get; set; }
        public bool IsKTPVerified { get; set; }
        public bool IsActive { get; set; }
        public string UserImageUrl { get; set; }
    }

    public class UpdateExternalUserDto 
    {
        public Guid Id { get; set; }
        public int AbpUserId { get; set; }
        public bool IsActive { get; set; }
        public bool IsKTPVerified { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public User LastModifierUser { get; set; }
    }

    public class RegisterExternalUserDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Channel { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string ShopName { get; set; }
        public string ShopImageurl { get; set; }
        public string KTPImageUrl { get; set; }
        public string UserImageUrl { get; set; }
        [Required]
        public string Address { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Handphone { get; set; }
        [Required]
        public string Jabatan { get; set; }
    }

    public class RegisterWithUploadExternalUserDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Channel { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string ShopName { get; set; }
        public IFormFile ShopImageFile { get; set; }
        [Required]
        public string Address { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Handphone { get; set; }
        [Required]
        public string Jabatan { get; set; }
    }

    public class UpdateExternalUserImageDto
    {
        public string Username { get; set; }
        public string UserImageUrl { get; set; }
        public string KTPImageUrl { get; set; }
        public string ShopImageUrl { get; set; }

    }
}
