using Abp.Application.Services;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IHomeworkQuizHistoryAppService : IApplicationService
    {
        IQueryable<HomeworkQuizHistories> GetAll();
        Task<List<HomeworkQuizHistories>> GetAllAsync();
        List<HomeworkQuizHistories> GetAllByHomeworkQuiz(Guid homeworkQuizId);
        List<HomeworkQuizHistories> GetAllByUser(int idmpm);
        HomeworkQuizHistories GetById(Guid id);
        void Create(HomeworkQuizHistoryCreateDto input);
    }
}
