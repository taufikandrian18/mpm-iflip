using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface ILiveQuizAppService : IApplicationService
    {
        IQueryable<LiveQuizzes> GetAll();
        LiveQuizzes GetById(Guid id);
        void Create(LiveQuizzes input);
        void Update(LiveQuizzes input);
        void SoftDelete(Guid id, string username);
    }
}
