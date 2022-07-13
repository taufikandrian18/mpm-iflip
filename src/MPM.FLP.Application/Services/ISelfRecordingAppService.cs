using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface ISelfRecordingAppService : IApplicationService
    {
        IQueryable<SelfRecordings> GetAll();
        List<SelfRecordings> GetAllItems();
        SelfRecordings GetById(Guid id);
        void Create(SelfRecordings input);
        void Update(SelfRecordings input);
        void SoftDelete(Guid id, string username);
    }
}
