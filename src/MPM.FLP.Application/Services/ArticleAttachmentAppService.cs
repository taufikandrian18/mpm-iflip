using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Authorization;
using MPM.FLP.Authorization.Users;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class ArticleAttachmentAppService : FLPAppServiceBase, IArticleAttachmentAppService
    {
        private readonly IRepository<ArticleAttachments, Guid> _articleAttachmentRepository;

        public ArticleAttachmentAppService(IRepository<ArticleAttachments, Guid> articleAttachmentRepository)
        {
            _articleAttachmentRepository = articleAttachmentRepository;
        }
       
        public ArticleAttachments GetById(Guid id)
        {
            var articleAttachment = _articleAttachmentRepository.FirstOrDefault(x => x.Id == id);
            return articleAttachment;
        }

        public void Create(ArticleAttachments input)
        {
            _articleAttachmentRepository.Insert(input);
        }

        public void Update(ArticleAttachments input)
        {
            _articleAttachmentRepository.Update(input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var articleAttachment = _articleAttachmentRepository.FirstOrDefault(x => x.Id == id);
            articleAttachment.DeleterUsername = username;
            articleAttachment.DeletionTime = DateTime.Now;
            _articleAttachmentRepository.Update(articleAttachment);
        }
    }
}
