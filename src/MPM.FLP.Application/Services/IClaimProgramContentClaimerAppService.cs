using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IClaimProgramContentClaimerAppService : IApplicationService
    {
        ClaimProgramContentClaimers GetById(Guid Id);
        BaseResponse GetAll([FromQuery] Pagination request);
        void Create(ClaimProgramContentClaimersCreateDto input);
        void Update(ClaimProgramContentClaimersUpdateDto input);
        void SoftDelete(ClaimProgramContentClaimersDeleteDto input);
        void Approve(ApprovalContentClaimerDto input);
        BaseResponse Verify(VerifyClaimerDto input);
        List<ClaimerDto> ExportExcel(FilterGetClaimerDto request);
        void RedeemPrize(RedeemPrizeClaimerDto input);
        void ResendOTP(Guid Id);
        BaseResponse GetClaimByCurrentUser([FromQuery] Pagination request);
        int GetPointByCurrentUser(Guid campaignId);
    }
}
