using Abp.Application.Services;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface ISalesPeopleDevelopmentContestAppService : IApplicationService
    {
        IQueryable<SPDCMasterPoints> GetAllMasterPoint();
        IQueryable<SPDCPointHistories> GetAllPointHistory();
        void CreateMasterPoint(SPDCMasterPoints input);
        void CreatePointHisotry(SPDCPointHistories input);
        void UpdateMasterHistory(SPDCMasterPoints input);
        void UpdatePointHistory(SPDCPointHistories input);
        void SoftDeleteMasterPoint(Guid id, string username);
        void SoftDeletePointHistory(Guid id, string username);
        List<SPDCLeaderBoardDto> GetLeaderBoard();

    }
}
