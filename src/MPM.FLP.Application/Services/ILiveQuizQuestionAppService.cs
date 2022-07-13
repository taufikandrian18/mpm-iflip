using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface ILiveQuizQuestionAppService : IApplicationService
    {
        IQueryable<LiveQuizQuestions> GetAll();
        LiveQuizQuestions GetById(Guid id);
        void Create(LiveQuizQuestions input);
        void Update(LiveQuizQuestions input);
        void SoftDelete(Guid id, string username);
    }
}
