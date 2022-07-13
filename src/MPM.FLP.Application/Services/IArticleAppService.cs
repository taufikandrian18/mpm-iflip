using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IArticleAppService : IApplicationService
    {
        IQueryable<Articles> GetAll();
        ICollection<ArticleAttachments> GetAllAttachments(Guid id);
        List<Guid> GetAllIds(string channel);
        List<Guid> GetAllH1Ids(string channel);
        List<Guid> GetAllH2Ids(string channel);
        List<Guid> GetAllH3Ids(string channel);
        List<Guid> GetAllHC3Ids(string channel);
        Articles GetById(Guid id);
        void Create(Articles input);
        void Update(Articles input);
        void SoftDelete(Guid id, string username);
    }
}
