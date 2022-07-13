using Abp.Application.Services;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface ILiveQuizHistoryAppService : IApplicationService
    {
        IQueryable<LiveQuizHistories> GetAll();
        List<LiveQuizHistories> GetAllByLiveQuiz(Guid LiveQuizId);
        List<LiveQuizHistories> GetAllByUser(int idmpm);
        LiveQuizHistories GetById(Guid id);
        List<LiveQuizLeaderBoardDto> GetLeaderboard(Guid liveQuizId, int IDMPM);
        void Create(LiveQuizHistoryCreateDto input);
    }
}
