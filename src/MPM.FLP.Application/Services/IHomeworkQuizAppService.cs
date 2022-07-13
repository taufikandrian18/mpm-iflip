using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IHomeworkQuizAppService : IApplicationService
    {
        IQueryable<HomeworkQuizzes> GetAll();
        HomeworkQuizzes GetById(Guid id);
        void Create(HomeworkQuizzes input);
        void Update(HomeworkQuizzes input);
        void SoftDelete(Guid id, string username);
    }
}
