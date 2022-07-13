using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IGuideAppService : IApplicationService
    {
        IQueryable<Guides> GetAll();
        ICollection<GuideAttachments> GetAllAttachments(Guid id);
        List<Guid> GetAllIds();
        List<Guid> GetAllTechnicalGuideIds(string channel);
        List<Guid> GetAllServiceGuideIds(Guid categoryId, string channel);
        Guides GetById(Guid id);
        void Create(Guides input, string PageName);
        void Update(Guides input, string PageName);
        void SoftDelete(Guid Id, string username, string PageName);
    }
}
