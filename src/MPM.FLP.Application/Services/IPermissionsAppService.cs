using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Linq;

namespace MPM.FLP.Services
{
    public interface IPermissionsAppService : IApplicationService
    {
        IQueryable<TBPermissions> GetAll();
        TBPermissions GetById(Guid id);
        void Create(TBPermissions input);
        void Update(TBPermissions input);
        void SoftDelete(Guid id, string username);
    }
}
