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
    public class SelfRecordingResultDetailAppService : FLPAppServiceBase, ISelfRecordingResultDetailAppService
    {
        private readonly IRepository<SelfRecordingResultDetails, Guid> _selfRecordingResultDetailRepository;

        public SelfRecordingResultDetailAppService(IRepository<SelfRecordingResultDetails, Guid> selfRecordingResultDetailRepository)
        {
            _selfRecordingResultDetailRepository = selfRecordingResultDetailRepository;
        }

        public void Create(SelfRecordingResultDetails input)
        {
            _selfRecordingResultDetailRepository.Insert(input);
        }

        public IQueryable<SelfRecordingResultDetails> GetAll()
        {
            return _selfRecordingResultDetailRepository.GetAll();
        }

        public SelfRecordingResultDetails GetById(Guid id)
        {
            return _selfRecordingResultDetailRepository.GetAll().FirstOrDefault(x => x.Id == id);
        }

        public void SoftDelete(Guid id, string username)
        {
            var SelfRecordingResultDetail = _selfRecordingResultDetailRepository.FirstOrDefault(x => x.Id == id);
            SelfRecordingResultDetail.DeleterUsername = username;
            SelfRecordingResultDetail.DeletionTime = DateTime.Now;
            _selfRecordingResultDetailRepository.Update(SelfRecordingResultDetail);
        }

        public void Update(SelfRecordingResultDetails input)
        {
            _selfRecordingResultDetailRepository.Update(input);
        }
    }
}
