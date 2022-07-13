using Abp.Application.Services;
using Microsoft.AspNetCore.Http;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IMobileAccountAppService : IApplicationService
    {
        Task<ServiceResult> InternalUserRegister(RegisterInternalUserDto input);
        Task<ServiceResult> ExternalUserRegister(RegisterExternalUserDto input);
        Task<ServiceResult> ExternalUserRegisterWithUpload(RegisterWithUploadExternalUserDto input);
        Task<ServiceResult> MobileResetPassword(MobileResetPasswordDto input);
        ServiceResult MobileValidateUsername(String username);
        Task<ServiceResult> UpdateProfilePicture(IFormFile ImageFile);
        TemporaryMobileDashboardDto GetTemporaryMobileDashboard(string channel);
        Task<LoginExtensionDto> GetUserProfile(string username);
        Task<ServiceResult> SendOTP(SendOTPDto input);
        ServiceResult ConfirmOTP(ConfirmOTPDto input);
        ServiceResult CheckUsername(string username);
        ServiceResult UpdateImageExternalUser(UpdateExternalUserImageDto input);
        List<ArticleDummy> GetListArticleDummy();
        bool IsAbleToRegisterTbsm(TbsmUserDto input);
        Task<ServiceResult> TbsmUserRegister(RegisterTbsmUserDto input);
    }
}
