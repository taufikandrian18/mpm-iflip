using Abp.Application.Services;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IForumPostAppService : IApplicationService
    {
        IQueryable<ForumPosts> GetAll();
        List<Guid> GetAllIdByForumThread(Guid forumThreadId);
        ForumPosts GetById(Guid id);
        void Create(ForumPostCreateDto input);
        void Update(ForumPosts input);
        void SoftDelete(Guid id, string username);
    }
}
