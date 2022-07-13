using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface ISalesTalkAttachmentAppService : IApplicationService
    {
        SalesTalkAttachments GetById(Guid id);
        void Create(SalesTalkAttachments input);
        void Update(SalesTalkAttachments input);
        void SoftDelete(Guid id, string username);
    }
}
