using Abp.Application.Services;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System.Collections.Generic;

namespace MPM.FLP.Services
{
    public interface IContentBankReportingAppService : IApplicationService
    {
        BaseResponse GetAllDownload(Pagination request);
        BaseResponse GetAllSosmed(Pagination request);
        List<ContentBankSosmedDto> ExportExcelSosmed(string channel, string search);
        List<ContentBankDownloadDto> ExportExcelDownload(string channel, string search);
    }
}
