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
    public class HomeworkQuizQuestionAppService : FLPAppServiceBase, IHomeworkQuizQuestionAppService
    {
        private readonly IRepository<HomeworkQuizQuestions, Guid> _homeworkQuizQuestionRepository;

        public HomeworkQuizQuestionAppService(IRepository<HomeworkQuizQuestions, Guid> homeworkQuizQuestionRepository)
        {
            _homeworkQuizQuestionRepository = homeworkQuizQuestionRepository;
            
        }

        public void Create(HomeworkQuizQuestions input)
        {
            _homeworkQuizQuestionRepository.Insert(input);
        }

        public IQueryable<HomeworkQuizQuestions> GetAll()
        {
            return _homeworkQuizQuestionRepository.GetAll().Where(Q=> !Q.DeletionTime.HasValue);
        }

        public HomeworkQuizQuestions GetById(Guid id)
        {
            return _homeworkQuizQuestionRepository.GetAll().FirstOrDefault(x => x.Id == id);
        }

        public void SoftDelete(Guid id, string username)
        {
            var homeworkQuizQuestion = _homeworkQuizQuestionRepository.FirstOrDefault(x => x.Id == id);
            homeworkQuizQuestion.DeleterUsername = username;
            homeworkQuizQuestion.DeletionTime = DateTime.UtcNow.AddHours(7);
            _homeworkQuizQuestionRepository.Update(homeworkQuizQuestion);
        }

        public void Update(HomeworkQuizQuestions input)
        {
            _homeworkQuizQuestionRepository.Update(input);
        }
    }
}
