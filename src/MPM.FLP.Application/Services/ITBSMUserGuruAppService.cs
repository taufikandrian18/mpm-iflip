using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;

namespace MPM.FLP.Services
{
    public interface ITBSMUserGuruAppService : IApplicationService
    {
        BaseResponse GetAll([FromQuery] Pagination request);
        TBSMUserGurus GetById(Guid id);
        List<TBSMUserGurus> GetBySekolahId(Guid id);
        List<TBSMUserGurus> GetByNpsn(string NPSN);
        List<TBSMUserGurus> GetByNIP(string NIP);

        void Create(TBSMUserGurusCreateDto input);
        void Update(TBSMUserGurusUpdateDto input);
        void SoftDelete(TBSMUserGurusDeleteDto input);
    }
}
