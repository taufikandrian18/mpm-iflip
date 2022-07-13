using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IHomeworkQuizAnswerAppService : IApplicationService
    {
        IQueryable<HomeworkQuizAnswers> GetAll();
        List<HomeworkQuizAnswers> GetByHomeworkQuizHistory(Guid homeworkQuizHistoryId);
    }
}
