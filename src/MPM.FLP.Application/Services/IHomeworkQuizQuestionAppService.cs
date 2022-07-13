using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IHomeworkQuizQuestionAppService : IApplicationService
    {
        IQueryable<HomeworkQuizQuestions> GetAll();
        HomeworkQuizQuestions GetById(Guid id);
        void Create(HomeworkQuizQuestions input);
        void Update(HomeworkQuizQuestions input);
        void SoftDelete(Guid id, string username);
    }
}
