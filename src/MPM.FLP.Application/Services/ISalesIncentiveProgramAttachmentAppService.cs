using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services
{
    public interface ISalesIncentiveProgramAttachmentAppService : IApplicationService
    {
        SalesIncentiveProgramAttachments GetById(Guid id);
        void Create(SalesIncentiveProgramAttachments input);
        void Update(SalesIncentiveProgramAttachments input);
        void SoftDelete(Guid id, string username);
    }
}
