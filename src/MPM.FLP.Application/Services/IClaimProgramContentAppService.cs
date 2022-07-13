using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;

namespace MPM.FLP.Services
{
    public interface IClaimProgramContentAppService : IApplicationService
    {
        BaseResponse GetAllAdmin([FromQuery] Pagination request);
        BaseResponse GetUser([FromQuery] Pagination request);
        BaseResponse GetAllClaimByUser([FromQuery] Pagination request);
        ClaimProgramContents GetById(Guid Id);
        void Create(ClaimProgramContentsCreateDto input);
        void Update(ClaimProgramContentsUpdateDto input);
        void UpdateAttachment(ClaimProgramContentAttachmentsUpdateDto input);
        void SoftDelete(ClaimProgramContentsDeleteDto input);
        BaseResponse GetAllByCampaign([FromQuery] Pagination request);
        //List<ClaimProgramContentDownloadDto> ExportExcel(DateTime StartDate, DateTime EndDate);
    }
}
