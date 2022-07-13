using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IProgramAttachmentAppService : IApplicationService
    {
        ProgramAttachments GetById(Guid id);
        void Create(ProgramAttachments input);
        void Update(ProgramAttachments input);
        void SoftDelete(Guid id, string username);
    }
}
