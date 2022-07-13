using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface ISalesTalkAppService : IApplicationService
    {
        IQueryable<SalesTalks> GetAll();
        ICollection<SalesTalkAttachments> GetAllAttachments(Guid id);
        List<Guid> GetAllIds();
        SalesTalks GetById(Guid id);
        void Create(SalesTalks input);
        void Update(SalesTalks input);
        void SoftDelete(Guid id, string username);
    }
}
