using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IServiceProgramAppService : IApplicationService
    {
        IQueryable<ServicePrograms> GetAll();
        IQueryable<ServicePrograms> GetAllBackoffice();
        ICollection<ServiceProgramAttachments> GetAllAttachments(Guid id);
        List<Guid> GetAllIds(string channel);
        ServicePrograms GetById(Guid id);
        void Create(ServicePrograms input);
        void Update(ServicePrograms input);
        void SoftDelete(Guid id, string username);
    }
}
