using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class HomeworkQuizHistoryAppService : FLPAppServiceBase, IHomeworkQuizHistoryAppService
    {
        private readonly IRepository<HomeworkQuizHistories, Guid> _homeworkQuizHistoryRepository;

        public HomeworkQuizHistoryAppService(IRepository<HomeworkQuizHistories, Guid> homeworkQuizHistoryRepository)
        {
            _homeworkQuizHistoryRepository = homeworkQuizHistoryRepository;
        }

        public void Create(HomeworkQuizHistoryCreateDto input)
        {

            /*var questions = _homeworkQuizHistoryRepository.GetAll().Where(x => x.HomeworkQuizId == input.HomeworkQuizId).Include(x => x.HomeworkQuiz).ThenInclude(x => x.HomeworkQuizQuestions).FirstOrDefault();

            var questionCount = questions.HomeworkQuiz.HomeworkQuizQuestions.Count();

            if (questionCount < input.CorrectAnswer) {
                input.CorrectAnswer = questionCount;
                input.WrongAnswer = 0;
                input.Score = 100;
            }*/

            var id = Guid.NewGuid();
            HomeworkQuizHistories homeworkHistory = new HomeworkQuizHistories()
            {
                Id = id,
                HomeworkQuizId = input.HomeworkQuizId,
                IDMPM = input.IDMPM,
                Name = input.Name,
                Jabatan = input.Jabatan,
                Kota = input.Kota,
                Dealer = input.Dealer,
                CorrectAnswer = input.CorrectAnswer,
                WrongAnswer = input.WrongAnswer,
                Score = input.Score,
                CreatorUsername = input.CreatorUsername,
                CreationTime = DateTime.UtcNow.AddHours(7),
                HomeworkQuizAnswers = new List<HomeworkQuizAnswers>()
            };
            foreach (var answer in input.HomewrorkAnswer)
            {
                var answers = new HomeworkQuizAnswers()
                {
                    Id = Guid.NewGuid(),
                    HomeworkQuizHistoryId = id,
                    Question = answer.Question,
                    Answer = answer.Answer,
                    CreatorUsername = input.CreatorUsername,
                    CreationTime = DateTime.UtcNow.AddHours(7)
                };
                homeworkHistory.HomeworkQuizAnswers.Add(answers);
            }


            _homeworkQuizHistoryRepository.Insert(homeworkHistory);


        }

        public IQueryable<HomeworkQuizHistories> GetAll()
        {
            return _homeworkQuizHistoryRepository.GetAll()
                        .Include(x => x.HomeworkQuiz)
                        .ThenInclude(x => x.HomeworkQuizQuestions)
                        .Include(x => x.HomeworkQuizAnswers);
        }

        public async  Task<List<HomeworkQuizHistories>> GetAllAsync()
        {
            return await _homeworkQuizHistoryRepository.GetAll().Include(x => x.HomeworkQuizAnswers).ToListAsync();
        }

        public List<HomeworkQuizHistories> GetAllByHomeworkQuiz(Guid homeworkQuizId)
        {
            return _homeworkQuizHistoryRepository.GetAll().Where(x => x.HomeworkQuizId == homeworkQuizId).ToList();
        }

        public List<HomeworkQuizHistories> GetAllByUser(int idmpm)
        {
            return _homeworkQuizHistoryRepository.GetAll().Include(x => x.HomeworkQuiz).Where(x => x.IDMPM == idmpm).ToList();
        }

        public HomeworkQuizHistories GetById(Guid id)
        {
            return _homeworkQuizHistoryRepository.GetAll().FirstOrDefault(x => x.Id == id);
        }
    }
}
