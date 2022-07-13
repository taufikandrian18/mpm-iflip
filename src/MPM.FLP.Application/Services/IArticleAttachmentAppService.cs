using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IArticleAttachmentAppService : IApplicationService
    {
        ArticleAttachments GetById(Guid id);
        void Create(ArticleAttachments input);
        void Update(ArticleAttachments input);
        void SoftDelete(Guid id, string username);
    }
}
