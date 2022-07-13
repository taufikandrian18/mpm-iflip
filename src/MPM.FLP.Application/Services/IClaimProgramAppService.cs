using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IClaimProgramAppService : IApplicationService
    {
        IQueryable<ClaimPrograms> GetAll();
        ICollection<ClaimProgramAttachments> GetAllAttachments(Guid id);
        List<Guid> GetAllIds(string channel);
        ClaimPrograms GetById(Guid id);
        void Create(ClaimPrograms input);
        void Update(ClaimPrograms input);
        void SoftDelete(Guid id, string username);
    }
}
