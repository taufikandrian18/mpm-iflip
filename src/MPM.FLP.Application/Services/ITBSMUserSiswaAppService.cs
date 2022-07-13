using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;

namespace MPM.FLP.Services
{
    public interface ITBSMUserSiswaAppService : IApplicationService
    {
        BaseResponse GetAll([FromQuery] Pagination request);
        TBSMUserSiswas GetById(Guid id);
        List<TBSMUserSiswas> GetBySekolah(Guid id);
        List<TBSMUserSiswas> GetByNpsn(string NPSN);
        List<TBSMUserSiswas> GetByNISN(string NISN);
        void Create(TBSMUserSiswasCreateDto input);
        void Update(TBSMUserSiswasUpdateDto input);
        void SoftDelete(TBSMUserSiswasDeleteDto input);
    }
}
