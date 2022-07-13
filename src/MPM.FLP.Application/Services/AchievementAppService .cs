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
    public class AchievementAppService : FLPAppServiceBase, IAchievementAppService
    {
        private readonly IRepository<Achievements, Guid> _achievementRepository;

        public AchievementAppService(IRepository<Achievements, Guid> achievementRepository)
        {
            _achievementRepository = achievementRepository;
        }

        public IQueryable<Achievements> GetAll()
        {
            return _achievementRepository.GetAll().Where(x=> x.DeletionTime == null);
        }

        public IQueryable<Guid> GetAllIds()
        {
            return _achievementRepository.GetAll().Select(x => x.Id);
        }

        public Achievements GetById(Guid id)
        {
            var achievements = _achievementRepository.FirstOrDefault(x => x.Id == id);

            return achievements;
        }

        public void Create(Achievements input)
        {
            _achievementRepository.Insert(input);
        }

        public void Update(Achievements input)
        {
            _achievementRepository.Update(input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var achievement =  _achievementRepository.FirstOrDefault(x => x.Id == id);
            achievement.DeleterUsername = username;
            achievement.DeletionTime = DateTime.Now;
            _achievementRepository.Update(achievement);
        }
    }
}
