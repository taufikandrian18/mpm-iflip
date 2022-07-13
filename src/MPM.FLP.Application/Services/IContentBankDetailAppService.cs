using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;

namespace MPM.FLP.Services
{
    public interface IContentBankDetailAppService : IApplicationService
    {
        List<ContentBankDetails> GetByContentBankId(Guid Id);
        void Update(ContentBanksDetailsUpdateDto input);
        List<ContentBankDetails> DownloadByContentBankId(List<Guid> Ids);
        ContentBankDetails DownloadById(Guid Id);
        void ReadContentBankDetail(Guid Id);
        ContentBanksDetailsByUserDto GetContentBankDetailByUser(Guid Id);
        void SoftDelete(ContentBanksDetailsDeleteDto input);
    }
}
