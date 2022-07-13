using Abp.Application.Services;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IForumThreadAppService : IApplicationService
    {
        IQueryable<ForumThreads> GetAll();
        List<Guid> GetAllIdByDealer(string kodeDealerMPM, string channel);
        ForumThreads GetById(Guid id);
        void Create(ForumThreadCreateDto input);
        void Update(ForumThreads input);
        void SoftDelete(Guid id, string username);
    }
}
