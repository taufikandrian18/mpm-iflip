using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.LogActivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class ClubCommunityCategoryAppService : FLPAppServiceBase, IClubCommunityCategoryAppService
    {
        private readonly IRepository<ClubCommunityCategories, Guid> _clubCommunityCategoryRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;
        public ClubCommunityCategoryAppService(

            IRepository<ClubCommunityCategories, Guid> ClubCommunityRepository,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService)
        {
            _clubCommunityCategoryRepository = ClubCommunityRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public IQueryable<ClubCommunityCategories> GetAll()
        {
            return _clubCommunityCategoryRepository.GetAll().Where(x => x.DeletionTime == null);
        }

        public List<ClubCommunityCategories> GetAllClubCommunityCategoryItems()
        {
            return _clubCommunityCategoryRepository.GetAll().Include(x => x.ClubCommunities).ToList();
        }

        public ClubCommunityCategories GetById(Guid id)
        {
            var clubCommunityCategory = _clubCommunityCategoryRepository.GetAll()
                                                    .Include(x => x.ClubCommunities)
                                                    .FirstOrDefault(x => x.Id == id);

            return clubCommunityCategory;
        }

        public void Create(ClubCommunityCategories input)
        {
            var categoryId = _clubCommunityCategoryRepository.InsertAndGetId(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Club Community Category", categoryId, input.Name, LogAction.Create.ToString(), null, input);
        }

        public void Update(ClubCommunityCategories input)
        {
            var oldObject = _clubCommunityCategoryRepository.GetAll().AsNoTracking().Include(x => x.ClubCommunities).FirstOrDefault(x => x.Id == input.Id);
            _clubCommunityCategoryRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Club Community Category", input.Id, input.Name, LogAction.Update.ToString(), oldObject, input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var clubCommunityCategory = _clubCommunityCategoryRepository.FirstOrDefault(x => x.Id == id);
            var oldObject = _clubCommunityCategoryRepository.GetAll().AsNoTracking().FirstOrDefault(x => x.Id == id);
            clubCommunityCategory.DeleterUsername = username;
            clubCommunityCategory.DeletionTime = DateTime.Now;
            _clubCommunityCategoryRepository.Update(clubCommunityCategory);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "Club Community Category", id, clubCommunityCategory.Name, LogAction.Delete.ToString(), oldObject, clubCommunityCategory);

        }
    }
}
