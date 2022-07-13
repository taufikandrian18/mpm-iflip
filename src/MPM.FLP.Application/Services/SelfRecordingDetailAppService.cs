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
    public class SelfRecordingDetailAppService : FLPAppServiceBase, ISelfRecordingDetailAppService
    {
        private readonly IRepository<SelfRecordingDetails, Guid> _selfRecordingDetailRepository;

        public SelfRecordingDetailAppService(IRepository<SelfRecordingDetails, Guid> selfRecordingDetailRepository)
        {
            _selfRecordingDetailRepository = selfRecordingDetailRepository;
        }

        public void Create(SelfRecordingDetails input)
        {
            _selfRecordingDetailRepository.Insert(input);
        }

        public IQueryable<SelfRecordingDetails> GetAll()
        {
            return _selfRecordingDetailRepository.GetAll();
        }

        public List<SelfRecordingDetails> GetAllItemBySelfRecording(Guid SelfRecordingId)
        {
            return _selfRecordingDetailRepository.GetAll().Where(x => x.SelfRecordingId == SelfRecordingId).ToList();
        }

        public SelfRecordingDetails GetById(Guid id)
        {
            return _selfRecordingDetailRepository.GetAll().FirstOrDefault(x => x.Id == id);
        }

        public void SoftDelete(Guid id, string username)
        {
            var SelfRecordingDetail = _selfRecordingDetailRepository.FirstOrDefault(x => x.Id == id);
            SelfRecordingDetail.DeleterUsername = username;
            SelfRecordingDetail.DeletionTime = DateTime.Now;
            _selfRecordingDetailRepository.Update(SelfRecordingDetail);
        }

        public void Update(SelfRecordingDetails input)
        {
            _selfRecordingDetailRepository.Update(input);
        }
    }
}
