using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.LogActivity;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class SalesPeopleDevelopmentContestAppService : FLPAppServiceBase, ISalesPeopleDevelopmentContestAppService
    {
        private readonly IRepository<SPDCMasterPoints, Guid> _spdcMasterPointRepository;
        private readonly IRepository<SPDCPointHistories, Guid> _spdcPointHistoryRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;
        public SalesPeopleDevelopmentContestAppService(
                                                       IRepository<SPDCMasterPoints, Guid> spdcMasterPointRepository,
                                                       IRepository<SPDCPointHistories, Guid> spdcPointHistoryRepository,
                                                       IAbpSession abpSession,
                                                       LogActivityAppService logActivityAppService)
        {
            _spdcMasterPointRepository = spdcMasterPointRepository;
            _spdcPointHistoryRepository = spdcPointHistoryRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public void CreateMasterPoint(SPDCMasterPoints input)
        {
            var masterPointId = _spdcMasterPointRepository.InsertAndGetId(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Master Point", masterPointId, input.Title, LogAction.Create.ToString(), null, input);

        }

        public void CreatePointHisotry(SPDCPointHistories input)
        {
            _spdcPointHistoryRepository.Insert(input);
        }

        public IQueryable<SPDCMasterPoints> GetAllMasterPoint()
        {
            return _spdcMasterPointRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername));
        }

        public IQueryable<SPDCPointHistories> GetAllPointHistory()
        {
            return _spdcPointHistoryRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).Include(x=>x.InternalUsers).Include(x=>x.SPDCMasterPoints);
        }

        public List<SPDCLeaderBoardDto> GetLeaderBoard()
        {
            int tahun = DateTime.UtcNow.AddHours(7).Year;
            List<SPDCLeaderBoardDto> leaderBoards = new List<SPDCLeaderBoardDto>();
            List<SPDCPointHistories> histories = _spdcPointHistoryRepository.GetAll()
                                .Include(x => x.SPDCMasterPoints).Include(x => x.InternalUsers)
                                .Where(x => x.Periode.Year == tahun).ToList();
            List<SPDCMasterPoints> masterPoints = _spdcMasterPointRepository.GetAll().ToList();
            List<int> ids = histories.GroupBy(x => x.IDMPM).Select(x => x.First().IDMPM).ToList();

            foreach (var id in ids) 
            {
                List<SPDCLeaderBoardDetailDto> detailPoint = histories.Where(x => x.IDMPM == id && string.IsNullOrEmpty(x.DeleterUsername))
                    .GroupBy(x => x.SPDCMasterPointId).Select(x => new SPDCLeaderBoardDetailDto()
                {
                    Name = x.First().SPDCMasterPoints.Title,
                    TotalPoint = x.Sum(y => y.Point),
                    Weight = (double)x.First().SPDCMasterPoints.Weight,
                    Point = x.Sum(y => y.Point) * (double)x.First().SPDCMasterPoints.Weight
                }).ToList();

                var namaFLP = histories.Where(x => x.IDMPM == id).FirstOrDefault().InternalUsers.Nama;

                SPDCLeaderBoardDto leaderBoard = new SPDCLeaderBoardDto() 
                {
                    IDMPM = id,
                    NamaFLP = namaFLP,
                    TotalPoint = detailPoint.Sum(x => x.Point),
                    DetailPoint = detailPoint
                };

                leaderBoards.Add(leaderBoard);
            }


            return leaderBoards.OrderByDescending(x => x.TotalPoint).ToList();
        }

        public void SoftDeleteMasterPoint(Guid id, string username)
        {
            var spdcMasterPoint = _spdcMasterPointRepository.FirstOrDefault(x => x.Id == id);
            var oldObject = _spdcMasterPointRepository.GetAll().AsNoTracking().FirstOrDefault(x => x.Id == id); 
            spdcMasterPoint.DeleterUsername = username;
            spdcMasterPoint.DeletionTime = DateTime.Now;
            _spdcMasterPointRepository.Update(spdcMasterPoint);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "Master Point", id, spdcMasterPoint.Title, LogAction.Delete.ToString(), oldObject, spdcMasterPoint);

        }

        public void SoftDeletePointHistory(Guid id, string username)
        {
            var spdcPointHistory = _spdcPointHistoryRepository.FirstOrDefault(x => x.Id == id);
            spdcPointHistory.DeleterUsername = username;
            spdcPointHistory.DeletionTime = DateTime.Now;
            _spdcPointHistoryRepository.Update(spdcPointHistory);
        }

        public void UpdateMasterHistory(SPDCMasterPoints input)
        {
            var oldObject = _spdcMasterPointRepository.GetAll().AsNoTracking().Include(x => x.SPDCPointHistories).FirstOrDefault(x => x.Id == input.Id);
            _spdcMasterPointRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Master Point", input.Id, input.Title, LogAction.Update.ToString(), oldObject, input);
        }

        public void UpdatePointHistory(SPDCPointHistories input)
        {
            _spdcPointHistoryRepository.Update(input);
        }
    }
}
