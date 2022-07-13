using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class LiveQuizHistoryAppService : FLPAppServiceBase, ILiveQuizHistoryAppService
    {
        private readonly IRepository<LiveQuizHistories, Guid> _liveQuizHistoryRepository;

        public LiveQuizHistoryAppService(IRepository<LiveQuizHistories, Guid> liveQuizHistoryRepository)
        {
            _liveQuizHistoryRepository = liveQuizHistoryRepository;
        }

        public void Create(LiveQuizHistoryCreateDto input)
        {

            var id = Guid.NewGuid();
            var history = _liveQuizHistoryRepository.GetAll().Where(x => x.LiveQuizId == input.LiveQuizId).ToList();
            var multipleFactor = 0;
            if (history.Count >= 100) { multipleFactor = 1; }
            else { multipleFactor = 100 - history.Count; }

            LiveQuizHistories LiveHistory = new LiveQuizHistories()
            {
                Id = id,
                LiveQuizId = input.LiveQuizId,
                IDMPM = input.IDMPM,
                Name = input.Name,
                Jabatan = input.Jabatan,
                Kota = input.Kota,
                Dealer = input.Dealer,
                CorrectAnswer = input.CorrectAnswer,
                WrongAnswer = input.WrongAnswer,
                Score = input.CorrectAnswer * multipleFactor,
                CreatorUsername = input.CreatorUsername,
                CreationTime = DateTime.UtcNow.AddHours(7),
                LiveQuizAnswers = new List<LiveQuizAnswers>()
            };
            foreach (var answer in input.LiveAnswer)
            {
                var answers = new LiveQuizAnswers()
                {
                    Id = Guid.NewGuid(),
                    LiveQuizHistoryId = id,
                    Question = answer.Question,
                    Answer = answer.Answer,
                    CreatorUsername = input.CreatorUsername,
                    CreationTime = DateTime.UtcNow.AddHours(7)
                };
                LiveHistory.LiveQuizAnswers.Add(answers);
            }

            _liveQuizHistoryRepository.Insert(LiveHistory);

        }

        public IQueryable<LiveQuizHistories> GetAll()
        {
            return _liveQuizHistoryRepository.GetAll().Include(x => x.LiveQuizAnswers);
        }

        public List<LiveQuizLeaderBoardDto> GetLeaderboard(Guid liveQuizId, int idmpm) 
        {
            var leaderboards = new List<LiveQuizLeaderBoardDto>();

            var top5 = _liveQuizHistoryRepository.GetAll().Where(x => x.LiveQuizId == liveQuizId)
                                                    .OrderByDescending(x => x.Score).Take(5).ToList();
            var user = top5.FirstOrDefault(x => x.IDMPM == idmpm);
            var userRank = 0;

            foreach (var rank in top5) 
            {
                userRank += 1;
                var leaderboard = new LiveQuizLeaderBoardDto() 
                {
                    Rank = userRank,
                    Name = rank.Name,
                    Score = rank.Score.Value
                };
                leaderboards.Add(leaderboard);
            }

            if (user == null) 
            {
                user = _liveQuizHistoryRepository.GetAll().OrderByDescending(x => x.Score)
                                                    .FirstOrDefault(x => x.LiveQuizId == liveQuizId && x.IDMPM == idmpm);

                if (user != null)
                {
                    userRank = _liveQuizHistoryRepository.GetAll().Where(x => x.LiveQuizId == liveQuizId && x.Score > user.Score)
                                            .ToList().Count + 1;
                    var leaderboard = new LiveQuizLeaderBoardDto() 
                    {
                        Rank = userRank,
                        Name = user.Name,
                        Score = user.Score.Value
                    };
                    leaderboards.Add(leaderboard);
                }
            }

            return leaderboards;
        }

        public List<LiveQuizHistories> GetAllByLiveQuiz(Guid LiveQuizId)
        {
            return _liveQuizHistoryRepository.GetAll().Where(x => x.LiveQuizId == LiveQuizId).ToList();
        }

        public List<LiveQuizHistories> GetAllByUser(int idmpm)
        {
            return _liveQuizHistoryRepository.GetAll().Include(x => x.LiveQuiz).Where(x => x.IDMPM == idmpm).ToList();
        }

        public LiveQuizHistories GetById(Guid id)
        {
            return _liveQuizHistoryRepository.GetAll().FirstOrDefault(x => x.Id == id);
        }
    }
}
