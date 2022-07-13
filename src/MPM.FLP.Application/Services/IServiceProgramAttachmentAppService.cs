using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IServiceProgramAttachmentAppService : IApplicationService
    {
        ServiceProgramAttachments GetById(Guid id);
        void Create(ServiceProgramAttachments input);
        void Update(ServiceProgramAttachments input);
        void SoftDelete(Guid id, string username);
    }
}
