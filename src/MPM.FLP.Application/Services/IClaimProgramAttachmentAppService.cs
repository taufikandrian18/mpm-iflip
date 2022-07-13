using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IClaimProgramAttachmentAppService : IApplicationService
    {
        ClaimProgramAttachments GetById(Guid id);
        void Create(ClaimProgramAttachments input);
        void Update(ClaimProgramAttachments input);
        void SoftDelete(Guid id, string username);
    }
}
