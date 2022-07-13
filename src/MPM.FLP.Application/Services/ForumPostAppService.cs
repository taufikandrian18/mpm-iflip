using Abp.Authorization;
using Abp.Domain.Repositories;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class ForumPostAppService : FLPAppServiceBase, IForumPostAppService
    {
        private readonly IRepository<ForumPosts, Guid> _forumPostRepository;
        private readonly IRepository<ForumThreads, Guid> _forumThreadRepository;

        public ForumPostAppService(IRepository<ForumPosts, Guid> forumPostRepository, 
                                   IRepository<ForumThreads, Guid> forumThreadRepository)
        {
            _forumPostRepository = forumPostRepository;
            _forumThreadRepository = forumThreadRepository;
        }

        public IQueryable<ForumPosts> GetAll()
        {
            return _forumPostRepository.GetAll();
        }

        public List<Guid> GetAllIdByForumThread(Guid forumThreadId)
        {
            var ids = _forumPostRepository.GetAll().Where(x => x.ForumThreadId == forumThreadId)
                .OrderBy(x => x.CreationTime).Select(x => x.Id).ToList();

            return ids;
        }

        public ForumPosts GetById(Guid id)
        {
            var ForumPost = _forumPostRepository.FirstOrDefault(x => x.Id == id);
            return ForumPost;
        }

        public void Create(ForumPostCreateDto input)
        {

            ForumPosts newPost = new ForumPosts() 
            {
                Id = Guid.NewGuid(),
                Contents = input.Contents,
                ForumThreadId = input.ForumThreadId,
                CreatorUsername = input.CreatorUsername,
                CreationTime = DateTime.UtcNow.AddHours(7)
            };
            _forumPostRepository.Insert(newPost);

            var forumThread = _forumThreadRepository.GetAll().FirstOrDefault(x => x.Id == input.ForumThreadId);
            if (forumThread != null) 
            {
                forumThread.LastModifierUsername = input.CreatorUsername;
                forumThread.LastModificationTime = DateTime.UtcNow.AddHours(7);
                _forumThreadRepository.Update(forumThread);
            }
        }

        public void Update(ForumPosts input)
        {
            _forumPostRepository.Update(input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var forumPost = _forumPostRepository.FirstOrDefault(x => x.Id == id);
            forumPost.DeleterUsername = username;
            forumPost.DeletionTime = DateTime.Now;
            _forumPostRepository.Update(forumPost);
        }

    }
}
