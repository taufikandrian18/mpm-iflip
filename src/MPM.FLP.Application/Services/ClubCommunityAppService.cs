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
    public class ClubCommunityAppService : FLPAppServiceBase, IClubCommunityAppService
    {
        private readonly IRepository<ClubCommunities, Guid> _clubCommunityRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;

        public ClubCommunityAppService(
            IRepository<ClubCommunities, Guid> clubCommunityRepository,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService)
        {
            _clubCommunityRepository = clubCommunityRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public IQueryable<ClubCommunities> GetAll()
        {
            return _clubCommunityRepository.GetAll().Where(x=> string.IsNullOrEmpty(x.DeleterUsername));
        }

        public ClubCommunities GetById(Guid id)
        {
            var clubCommunity = _clubCommunityRepository.GetAll().FirstOrDefault(x => x.Id == id);
            return clubCommunity;
        }

        public ClubCommunities GetByName(string name)
        {
            var clubCommunity = _clubCommunityRepository.GetAll().FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
            return clubCommunity;
        }

        public void Create(ClubCommunities input)
        {
            //minta tolong lognya diperiksa, karena bikin error pas create
            //_logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Club Community", input.Id, input.Name, LogAction.Create.ToString(), null, input);
            var clubId = _clubCommunityRepository.InsertAndGetId(input);
            //_logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Club Community", clubId, input.Name, LogAction.Create.ToString(), null, input);
        }

        public void Update(ClubCommunities input)
        {
            var oldObject = _clubCommunityRepository.GetAll().AsNoTracking().FirstOrDefault(x => x.Id == input.Id);
            _clubCommunityRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Club Community", input.Id, input.Name, LogAction.Update.ToString(), oldObject, input);

        }

        public void SoftDelete(Guid id, string username)
        {
            var clubCommunity = _clubCommunityRepository.FirstOrDefault(x => x.Id == id);
            var oldObject = _clubCommunityRepository.GetAll().AsNoTracking().FirstOrDefault(x => x.Id == id);
            clubCommunity.DeleterUsername = username;
            clubCommunity.DeletionTime = DateTime.Now;
            _clubCommunityRepository.Update(clubCommunity);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "Club Community", id, clubCommunity.Name, LogAction.Delete.ToString(), oldObject, clubCommunity);

        }
    }
}
