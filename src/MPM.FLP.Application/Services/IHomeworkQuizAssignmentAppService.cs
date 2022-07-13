using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IHomeworkQuizAssignmentAppService : IApplicationService
    {
        IQueryable<HomeworkQuizAssignments> GetAll();
        HomeworkQuizAssignments GetById(Guid id);
        List<HomeworkQuizAssignments> GetByUser(int idmpm);
        List<HomeworkQuizAssignments> GetByHomeworkQuiz(Guid homeworkQuizId);
        void Create(HomeworkQuizAssignments input);
        void Delete(Guid id);
    }
}
