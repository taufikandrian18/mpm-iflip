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
    public class LiveQuizQuestionAppService : FLPAppServiceBase, ILiveQuizQuestionAppService
    {
        private readonly IRepository<LiveQuizQuestions, Guid> _liveQuizQuestionRepository;

        public LiveQuizQuestionAppService(IRepository<LiveQuizQuestions, Guid> liveQuizQuestionRepository)
        {
            _liveQuizQuestionRepository = liveQuizQuestionRepository;
        }

        public void Create(LiveQuizQuestions input)
        {
            _liveQuizQuestionRepository.Insert(input);
        }

        public IQueryable<LiveQuizQuestions> GetAll()
        {
            return _liveQuizQuestionRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername));
        }

        public LiveQuizQuestions GetById(Guid id)
        {
            return _liveQuizQuestionRepository.GetAll().FirstOrDefault(x => x.Id == id);
        }

        public void SoftDelete(Guid id, string username)
        {
            var LiveQuizQuestion = _liveQuizQuestionRepository.FirstOrDefault(x => x.Id == id);
            LiveQuizQuestion.DeleterUsername = username;
            LiveQuizQuestion.DeletionTime = DateTime.UtcNow.AddHours(7);
            _liveQuizQuestionRepository.Update(LiveQuizQuestion);
        }

        public void Update(LiveQuizQuestions input)
        {
            _liveQuizQuestionRepository.Update(input);
        }
    }
}
