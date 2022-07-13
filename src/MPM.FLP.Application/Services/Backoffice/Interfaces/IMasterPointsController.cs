using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;

namespace MPM.FLP.Services.Backoffice
{
    public interface IMasterPointsController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        string Create(SPDCMasterPoints model);
        string Update(SPDCMasterPoints model);

        string DestroyBackoffice(Guid guid);
    }
}