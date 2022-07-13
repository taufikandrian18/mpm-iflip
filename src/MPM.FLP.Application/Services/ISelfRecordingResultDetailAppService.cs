using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface ISelfRecordingResultDetailAppService : IApplicationService
    {
        IQueryable<SelfRecordingResultDetails> GetAll();
        SelfRecordingResultDetails GetById(Guid id);
        void Create(SelfRecordingResultDetails input);
        void Update(SelfRecordingResultDetails input);
        void SoftDelete(Guid id, string username);
    }
}
