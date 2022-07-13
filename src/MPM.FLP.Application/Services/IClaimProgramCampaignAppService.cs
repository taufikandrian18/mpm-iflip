using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;

namespace MPM.FLP.Services
{
    public interface IClaimProgramCampaignAppService : IApplicationService
    {
        BaseResponse GetAllAdmin([FromQuery] Pagination request);
        BaseResponse GetActiveClaimProgram([FromQuery] Pagination request);
        ClaimProgramCampaigns GetById(Guid Id);
        void Create(ClaimProgramCampaignsCreateDto input);
        void Update(ClaimProgramCampaignsUpdateDto input);
        void SoftDelete(ClaimProgramCampaignsDeleteDto input);
    }
}
