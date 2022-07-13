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
    public class HomeworkQuizAnswerAppService : FLPAppServiceBase, IHomeworkQuizAnswerAppService
    {
        private readonly IRepository<HomeworkQuizAnswers, Guid> _homeworkQuizAnswerRepository;

        public HomeworkQuizAnswerAppService(IRepository<HomeworkQuizAnswers, Guid> homeworkQuizAnswerRepository)
        {
            _homeworkQuizAnswerRepository = homeworkQuizAnswerRepository;
        }

        public IQueryable<HomeworkQuizAnswers> GetAll()
        {
            return _homeworkQuizAnswerRepository.GetAll();
        }

        public List<HomeworkQuizAnswers> GetByHomeworkQuizHistory(Guid homeworkQuizHistoryId)
        {
            return _homeworkQuizAnswerRepository.GetAll().Where(x => x.HomeworkQuizHistoryId == homeworkQuizHistoryId).ToList();
        }
    }
}
