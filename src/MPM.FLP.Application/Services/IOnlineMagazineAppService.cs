using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IOnlineMagazineAppService : IApplicationService
    {
        IQueryable<OnlineMagazines> GetAll();
        OnlineMagazines GetById(Guid id);
        void Create(OnlineMagazines input);
        void Update(OnlineMagazines input);
        void SoftDelete(Guid id, string username);
    }
}
