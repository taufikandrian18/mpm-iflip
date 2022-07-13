using Abp.Authorization;
using Abp.Domain.Repositories;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class LiveQuizAnswerAppService : FLPAppServiceBase, ILiveQuizAnswerAppService
    {
        private readonly IRepository<LiveQuizAnswers, Guid> _liveQuizAnswerRepository;

        public LiveQuizAnswerAppService(IRepository<LiveQuizAnswers, Guid> liveQuizAnswerRepository)
        {
            _liveQuizAnswerRepository = liveQuizAnswerRepository;
        }

        public IQueryable<LiveQuizAnswers> GetAll()
        {
            return _liveQuizAnswerRepository.GetAll();
        }

        public List<LiveQuizAnswers> GetByLiveQuizHistory(Guid LiveQuizHistoryId)
        {
            return _liveQuizAnswerRepository.GetAll().Where(x => x.LiveQuizHistoryId == LiveQuizHistoryId).ToList();
        }
    }
}
