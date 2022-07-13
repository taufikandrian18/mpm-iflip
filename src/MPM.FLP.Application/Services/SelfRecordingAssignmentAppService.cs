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
    public class SelfRecordingAssignmentAppService : FLPAppServiceBase, ISelfRecordingAssignmentAppService
    {
        private readonly IRepository<SelfRecordingAssignments, Guid> _selfRecordingAssignmentRepository;

        public SelfRecordingAssignmentAppService(IRepository<SelfRecordingAssignments, Guid> selfRecordingAssignmentRepository)
        {
            _selfRecordingAssignmentRepository = selfRecordingAssignmentRepository;
        }

        public void Create(SelfRecordingAssignments input)
        {
            _selfRecordingAssignmentRepository.Insert(input);
        }

        public IQueryable<SelfRecordingAssignments> GetAll()
        {
            return _selfRecordingAssignmentRepository.GetAll();
        }

        public SelfRecordingAssignments GetById(Guid id)
        {
            return _selfRecordingAssignmentRepository.GetAll().FirstOrDefault(x => x.Id == id);
        }

        public void Delete(Guid id)
        {
            _selfRecordingAssignmentRepository.Delete(id);
        }
    }
}
