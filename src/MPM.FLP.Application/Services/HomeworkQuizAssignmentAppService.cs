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
    public class HomeworkQuizAssignmentAppService : FLPAppServiceBase, IHomeworkQuizAssignmentAppService
    {
        private readonly IRepository<HomeworkQuizAssignments, Guid> _homeworkQuizAssignmentRepository;

        public HomeworkQuizAssignmentAppService(IRepository<HomeworkQuizAssignments, Guid> homeworkQuizAssignmentRepository)
        {
            _homeworkQuizAssignmentRepository = homeworkQuizAssignmentRepository;
        }

        public void Create(HomeworkQuizAssignments input)
        {
            _homeworkQuizAssignmentRepository.Insert(input);
        }

        public IQueryable<HomeworkQuizAssignments> GetAll()
        {
            return _homeworkQuizAssignmentRepository.GetAll();
        }

        public List<HomeworkQuizAssignments> GetByHomeworkQuiz(Guid homeworkQuizId)
        {
            return _homeworkQuizAssignmentRepository.GetAll().Where(x => x.HomeworkQuizId == homeworkQuizId).ToList();
        }

        public HomeworkQuizAssignments GetById(Guid id)
        {
            return _homeworkQuizAssignmentRepository.GetAll().FirstOrDefault(x => x.Id == id);
        }

        public List<HomeworkQuizAssignments> GetByUser(int idmpm)
        {
            var data =  _homeworkQuizAssignmentRepository.GetAll().Where(x => x.IDMPM == idmpm 
                                                              && x.HomeworkQuiz.IsPublished
                                                              && DateTime.Now.Date >= x.HomeworkQuiz.StartDate.Date
                                                              && DateTime.Now.Date <= x.HomeworkQuiz.EndDate.Date
                                                              && string.IsNullOrEmpty(x.HomeworkQuiz.DeleterUsername))
                                                            .OrderBy(x => x.HomeworkQuiz.EndDate).ToList();
            return data;
        }

        public void Delete(Guid id)
        {
            _homeworkQuizAssignmentRepository.Delete(id);
        }
    }
}
