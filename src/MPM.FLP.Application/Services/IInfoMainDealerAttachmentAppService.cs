using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IInfoMainDealerAttachmentAppService : IApplicationService
    {
        InfoMainDealerAttachments GetById(Guid id);
        void Create(InfoMainDealerAttachments input);
        void Update(InfoMainDealerAttachments input);
        void SoftDelete(Guid id, string username);
    }
}
