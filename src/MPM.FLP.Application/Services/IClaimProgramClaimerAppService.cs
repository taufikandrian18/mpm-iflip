using Abp.Application.Services;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IClaimProgramClaimerAppService : IApplicationService
    {
        ClaimProgramClaimers GetById(Guid id);
        List<ClaimerDto> GetClaimers(Guid claimProgramId);
        Task<ServiceResult> Create(NewClaimerDto input);
        Task<ServiceResult> Update(UpdateClaimerDto input);
        void Approve(ApprovalClaimDto input);
        void RecreateOTP(Guid input);
        ServiceResult Verifiy(VerifyClaimDto input);
        void SoftDelete(Guid id, string username);
    }
}
