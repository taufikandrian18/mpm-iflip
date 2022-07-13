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
    public class LiveQuizAppService : FLPAppServiceBase, ILiveQuizAppService
    {
        private readonly IRepository<LiveQuizzes, Guid> _liveQuizRepository;
        private readonly IRepository<InternalUsers> _internalUserRepository;
        private readonly IAbpSession _abpSession;
        private readonly IRepository<LiveQuizHistories, Guid> _liveQuizHistoryRepository;
        private readonly LogActivityAppService _logActivityAppService;

        public LiveQuizAppService(IRepository<LiveQuizzes, Guid> liveQuizRepository,
                                      IAbpSession abpSession,
                                      IRepository<InternalUsers> internalUserRepository,
                                      IRepository<LiveQuizHistories, Guid> liveQuizHistoryRepository,
                                      LogActivityAppService logActivityAppService)
        {
            _liveQuizRepository = liveQuizRepository;
            _internalUserRepository = internalUserRepository;
            _abpSession = abpSession;
            _liveQuizHistoryRepository = liveQuizHistoryRepository;
            _logActivityAppService = logActivityAppService;
        }
        public void Create(LiveQuizzes input)
        {
            //_liveQuizRepository.Insert(input);
            var liveQuizId = _liveQuizRepository.InsertAndGetId(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Live Quiz", liveQuizId, input.Title, LogAction.Create.ToString(), null, input);

        }

        public IQueryable<LiveQuizzes> GetAll()
        {
            return _liveQuizRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername));
        }

        public LiveQuizzes GetById(Guid id)
        {
            var liveQuiz = _liveQuizRepository.GetAll().Include(x => x.LiveQuizQuestions).FirstOrDefault(x => x.Id == id);
            try
            {

                long currentUserId = _abpSession.UserId.Value;
                var interalUser = _internalUserRepository.GetAll().FirstOrDefault(x => x.AbpUserId.Value == currentUserId);
                if (interalUser != null)
                {
                    var participant = _liveQuizHistoryRepository.GetAll()
                        .Where(x => x.LiveQuizId == liveQuiz.Id && x.IDMPM == interalUser.IDMPM)
                        .FirstOrDefault();
                    if (participant != null)
                    {
                        liveQuiz.IsAlreadyTaken = true;
                    }
                    else { liveQuiz.IsAlreadyTaken = false; }

                }
                else { liveQuiz.IsAlreadyTaken = false; }
            }
            catch (Exception) { liveQuiz.IsAlreadyTaken = false; }
            return liveQuiz;
        }

        public void SoftDelete(Guid id, string username)
        {
            var LiveQuiz = _liveQuizRepository.FirstOrDefault(x => x.Id == id);
            var oldObject = _liveQuizRepository.GetAll().AsNoTracking().FirstOrDefault(x => x.Id == id);
            LiveQuiz.DeleterUsername = username;
            LiveQuiz.DeletionTime = DateTime.UtcNow.AddHours(7);
            _liveQuizRepository.Update(LiveQuiz);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "Live Quiz", id, LiveQuiz.Title, LogAction.Delete.ToString(), oldObject, LiveQuiz);

        }

        public void Update(LiveQuizzes input)
        {
            var oldObject = _liveQuizRepository.GetAll().AsNoTracking().Include(x => x.LiveQuizQuestions).FirstOrDefault(x => x.Id == input.Id);
            _liveQuizRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Live Quiz", input.Id, input.Title, LogAction.Update.ToString(), oldObject, input);

        }
    }
}
