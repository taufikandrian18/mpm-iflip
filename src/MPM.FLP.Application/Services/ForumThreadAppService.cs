using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class ForumThreadAppService : FLPAppServiceBase, IForumThreadAppService
    {
        private readonly IRepository<ForumThreads, Guid> _forumThreadRepository;

        public ForumThreadAppService(IRepository<ForumThreads, Guid> forumThreadRepository)
        {
            _forumThreadRepository = forumThreadRepository;
        }

        public IQueryable<ForumThreads> GetAll()
        {
            return _forumThreadRepository.GetAll();
        }

        public List<Guid> GetAllIdByDealer(string kodeDealerMPM, string channel)
        {
            var ids = _forumThreadRepository.GetAll().Where(x => x.KodeDealerMPM == kodeDealerMPM && x.Channel == channel)
                .OrderByDescending(x => x.LastModificationTime).Select(x => x.Id).ToList();

            return ids;
        }

        public ForumThreads GetById(Guid id)
        {
            var forumThread = _forumThreadRepository.GetAll().Include(x => x.ForumPosts).FirstOrDefault(x => x.Id == id);
            return forumThread;
        }

        public void Create(ForumThreadCreateDto input)
        {

            ForumThreads newThread = new ForumThreads()
            {
                Id = Guid.NewGuid(),
                Title = input.Title,
                Contents = input.Contents,
                KodeDealerMPM = input.KodeDealerMPM,
                Channel = input.Channel,
                CreatorUsername = input.CreatorUsername,
                CreationTime = DateTime.UtcNow.AddHours(7),
                LastModificationTime= DateTime.UtcNow.AddHours(7),
                LastModifierUsername = input.CreatorUsername,
            };
            _forumThreadRepository.Insert(newThread);

        }

        public void Update(ForumThreads input)
        {
            _forumThreadRepository.Update(input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var forumThread = _forumThreadRepository.FirstOrDefault(x => x.Id == id);
            forumThread.DeleterUsername = username;
            forumThread.DeletionTime = DateTime.Now;
            _forumThreadRepository.Update(forumThread);
        }


    }
}
