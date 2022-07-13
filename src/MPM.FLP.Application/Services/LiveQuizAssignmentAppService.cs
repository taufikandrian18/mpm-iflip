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
    public class LiveQuizAssignmentAppService : FLPAppServiceBase, ILiveQuizAssignmentAppService
    {
        private readonly IRepository<LiveQuizAssignments, Guid> _liveQuizAssignmentRepository;

        public LiveQuizAssignmentAppService(IRepository<LiveQuizAssignments, Guid> liveQuizAssignmentRepository)
        {
            _liveQuizAssignmentRepository = liveQuizAssignmentRepository;
        }

        public void Create(LiveQuizAssignments input)
        {
            _liveQuizAssignmentRepository.Insert(input);
        }

        public IQueryable<LiveQuizAssignments> GetAll()
        {
            return _liveQuizAssignmentRepository.GetAll();
        }

        public List<LiveQuizAssignments> GetByLiveQuiz(Guid LiveQuizId)
        {
            return _liveQuizAssignmentRepository.GetAll().Where(x => x.LiveQuizId == LiveQuizId).ToList();
        }

        public LiveQuizAssignments GetById(Guid id)
        {
            return _liveQuizAssignmentRepository.GetAll().FirstOrDefault(x => x.Id == id);
        }

        public List<LiveQuizAssignments> GetByUser(int idmpm)
        {
            return _liveQuizAssignmentRepository.GetAll().Where(x => x.IDMPM == idmpm
                                                              && x.LiveQuiz.IsPublished
                                                              && DateTime.Now.Date >= x.LiveQuiz.StartDate.Date
                                                              && string.IsNullOrEmpty(x.LiveQuiz.DeleterUsername))
                                                            .OrderByDescending(x => x.LiveQuiz.StartDate).ToList();
        }

        public void Delete(Guid id)
        {
            _liveQuizAssignmentRepository.Delete(id);
        }
    }
}
