using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface ISelfRecordingDetailAppService : IApplicationService
    {
        IQueryable<SelfRecordingDetails> GetAll();
        List<SelfRecordingDetails> GetAllItemBySelfRecording(Guid selfRecordingId);
        SelfRecordingDetails GetById(Guid id);
        void Create(SelfRecordingDetails input);
        void Update(SelfRecordingDetails input);
        void SoftDelete(Guid id, string username);
    }
}
