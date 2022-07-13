using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface ILiveQuizAssignmentAppService : IApplicationService
    {
        IQueryable<LiveQuizAssignments> GetAll();
        LiveQuizAssignments GetById(Guid id);
        List<LiveQuizAssignments> GetByUser(int idmpm);
        List<LiveQuizAssignments> GetByLiveQuiz(Guid LiveQuizId);
        void Create(LiveQuizAssignments input);
        void Delete(Guid id);
    }
}
