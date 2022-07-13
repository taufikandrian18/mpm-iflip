using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IProgramAppService : IApplicationService
    {
        IQueryable<Programs> GetAll();
        ICollection<ProgramAttachments> GetAllAttachments(Guid id);
        List<Guid> GetAllIds(string channel);
        Programs GetById(Guid id);
        void Create(Programs input);
        void Update(Programs input);
        void SoftDelete(Guid id, string username);
    }
}
