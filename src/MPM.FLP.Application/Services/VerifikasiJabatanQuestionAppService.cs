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
    public class VerifikasiJabatanQuestionAppService : FLPAppServiceBase, IVerfikasiJabatanQuestionAppService
    {
        private readonly IRepository<VerifikasiJabatanQuestions, Guid> _verifikasiJabatanQuestionRepository;

        public VerifikasiJabatanQuestionAppService(IRepository<VerifikasiJabatanQuestions, Guid> verifikasiJabatanQuestionRepository)
        {
            _verifikasiJabatanQuestionRepository = verifikasiJabatanQuestionRepository;
        }

        public void Create(VerifikasiJabatanQuestions input)
        {
            _verifikasiJabatanQuestionRepository.Insert(input);
        }

        public IQueryable<VerifikasiJabatanQuestions> GetAll()
        {
            return _verifikasiJabatanQuestionRepository.GetAll();
        }

        public VerifikasiJabatanQuestions GetById(Guid id)
        {
            return _verifikasiJabatanQuestionRepository.GetAll().FirstOrDefault(x => x.Id == id);
        }

        public void SoftDelete(Guid id, string username)
        {
            var verifikasiJabatanQuestion = _verifikasiJabatanQuestionRepository.FirstOrDefault(x => x.Id == id);
            verifikasiJabatanQuestion.DeleterUsername = username;
            verifikasiJabatanQuestion.DeletionTime = DateTime.UtcNow.AddHours(7);
            _verifikasiJabatanQuestionRepository.Update(verifikasiJabatanQuestion);
        }

        public void Update(VerifikasiJabatanQuestions input)
        {
            _verifikasiJabatanQuestionRepository.Update(input);
        }
    }
}
