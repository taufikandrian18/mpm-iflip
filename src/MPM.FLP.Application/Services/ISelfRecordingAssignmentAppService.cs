using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface ISelfRecordingAssignmentAppService : IApplicationService
    {
        IQueryable<SelfRecordingAssignments> GetAll();
        SelfRecordingAssignments GetById(Guid id);
        void Create(SelfRecordingAssignments input);
        void Delete(Guid id);
    }
}
