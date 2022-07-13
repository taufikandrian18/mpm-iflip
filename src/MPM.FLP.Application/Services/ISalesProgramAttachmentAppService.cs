using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services
{
    public interface ISalesProgramAttachmentAppService : IApplicationService
    {
        SalesProgramAttachments GetById(Guid id);
        void Create(SalesProgramAttachments input);
        void Update(SalesProgramAttachments input);
        void SoftDelete(Guid id, string username);
    }
}
