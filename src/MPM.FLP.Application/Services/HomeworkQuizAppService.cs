using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.LogActivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class HomeworkQuizAppService : FLPAppServiceBase, IHomeworkQuizAppService
    {
        private readonly IRepository<HomeworkQuizzes, Guid> _homeworkQuizRepository;
        private readonly IRepository<InternalUsers> _internalUserRepository;
        private readonly IAbpSession _abpSession;
        private readonly IRepository<HomeworkQuizHistories, Guid> _homeworkQuizHistoryRepository;
        private readonly LogActivityAppService _logActivityAppService;
        public HomeworkQuizAppService(IRepository<HomeworkQuizzes, Guid> homeworkQuizRepository,
                                      IAbpSession abpSession,
                                      IRepository<InternalUsers> internalUserRepository,
                                      IRepository<HomeworkQuizHistories, Guid> homeworkQuizHistoryRepository,
                                      LogActivityAppService logActivityAppService)
        {
            _homeworkQuizRepository = homeworkQuizRepository;
            _internalUserRepository = internalUserRepository;
            _abpSession = abpSession;
            _homeworkQuizHistoryRepository = homeworkQuizHistoryRepository;
            _logActivityAppService = logActivityAppService;
        }

        public void Create(HomeworkQuizzes input)
        {
            //_homeworkQuizRepository.Insert(input);
            var homeworkId = _homeworkQuizRepository.InsertAndGetId(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Homework Quiz", homeworkId, input.Title, LogAction.Create.ToString(), null, input);
        }

        public IQueryable<HomeworkQuizzes> GetAll()
        {
            return _homeworkQuizRepository.GetAll().Where(x=> x.DeletionTime == null);
        }

        public HomeworkQuizzes GetById(Guid id)
        {
            var homeworkQuiz = _homeworkQuizRepository.GetAll().Include(x => x.HomeworkQuizQuestions).FirstOrDefault(x => x.Id == id);
            try
            {
                long currentUserId = _abpSession.UserId.Value;
                var interalUser = _internalUserRepository.GetAll().FirstOrDefault(x => x.AbpUserId.Value == currentUserId);
                if (interalUser != null)
                {
                    var participant = _homeworkQuizHistoryRepository.GetAll()
                        .Where(x => x.HomeworkQuizId == homeworkQuiz.Id && x.IDMPM == interalUser.IDMPM && !x.DeletionTime.HasValue).FirstOrDefault();
                    if (participant != null)
                    {
                        homeworkQuiz.IsAlreadyTaken = true;
                    }
                    else { homeworkQuiz.IsAlreadyTaken = false; }
                }
                else { homeworkQuiz.IsAlreadyTaken = false; }
            }
            catch (Exception) { homeworkQuiz.IsAlreadyTaken = false; }
            return homeworkQuiz;
        }

        public void SoftDelete(Guid id, string username)
        {
            var homeworkQuiz = _homeworkQuizRepository.FirstOrDefault(x => x.Id == id);
            var oldObject = _homeworkQuizRepository.GetAll().AsNoTracking().FirstOrDefault(x => x.Id == id);
            homeworkQuiz.DeleterUsername = username;
            homeworkQuiz.DeletionTime = DateTime.UtcNow.AddHours(7);
            _homeworkQuizRepository.Update(homeworkQuiz);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "Homework Quiz", id, homeworkQuiz.Title, LogAction.Delete.ToString(), oldObject, homeworkQuiz);
        }

        public void Update(HomeworkQuizzes input)
        {
            var oldObject = _homeworkQuizRepository.GetAll().AsNoTracking().Include(x => x.HomeworkQuizQuestions).FirstOrDefault(x => x.Id == input.Id);
            _homeworkQuizRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Homework Quiz", input.Id, input.Title, LogAction.Update.ToString(), oldObject, input);
        }
    }
}
