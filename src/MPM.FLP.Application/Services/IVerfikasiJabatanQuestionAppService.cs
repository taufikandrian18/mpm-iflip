using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IVerfikasiJabatanQuestionAppService : IApplicationService
    {
        IQueryable<VerifikasiJabatanQuestions> GetAll();
        VerifikasiJabatanQuestions GetById(Guid id);
        void Create(VerifikasiJabatanQuestions input);
        void Update(VerifikasiJabatanQuestions input);
        void SoftDelete(Guid id, string username);
    }
}
