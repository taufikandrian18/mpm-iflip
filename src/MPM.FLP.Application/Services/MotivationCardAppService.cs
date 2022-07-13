using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.LogActivity;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class MotivationCardAppService : FLPAppServiceBase, IMotivationCardAppService
    {
        private readonly IRepository<MotivationCards, Guid> _motivationCardRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;
        public MotivationCardAppService(
            IRepository<MotivationCards, Guid> motivationCardRepository,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService)
        {
            _motivationCardRepository = motivationCardRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public IQueryable<MotivationCards> GetAll()
        {
            return _motivationCardRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername));
        }


        public List<string> GetAllImages()
        {
            return _motivationCardRepository.GetAll().Where(x => x.IsPublished && string.IsNullOrEmpty(x.DeleterUsername))
                    .OrderBy(x => x.Order).Select(x => x.StorageUrl).ToList();
        }

        public List<MotivationCardDto> GetAllTitleImages() 
        {
            return _motivationCardRepository.GetAll().Where(x => x.IsPublished && string.IsNullOrEmpty(x.DeleterUsername))
                    .OrderBy(x => x.Order)
                    .Select(x => new MotivationCardDto() { Id = x.Id, Title = x.Title, StorageUrl = x.StorageUrl })
                    .ToList();
        }

        public MotivationCards GetById(Guid id)
        {
            var motivationCard = _motivationCardRepository.GetAll().FirstOrDefault(x => x.Id == id);
            return motivationCard;
        }

        public void Create(MotivationCards input)
        {
            var motivationId = _motivationCardRepository.InsertAndGetId(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Motivation Cards", motivationId, input.Title, LogAction.Create.ToString(), null, input);

        }

        public void Update(MotivationCards input)
        {
            var oldObject = _motivationCardRepository.GetAll().AsNoTracking().FirstOrDefault(x => x.Id == input.Id);
            _motivationCardRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Motivation Cards", input.Id, input.Title, LogAction.Update.ToString(), oldObject, input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var motivationCard = _motivationCardRepository.FirstOrDefault(x => x.Id == id);
            var oldObject = _motivationCardRepository.GetAll().AsNoTracking().FirstOrDefault(x => x.Id == id);
            motivationCard.DeleterUsername = username;
            motivationCard.DeletionTime = DateTime.Now;
            _motivationCardRepository.Update(motivationCard);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "Motivation Cards", id, motivationCard.Title, LogAction.Delete.ToString(), oldObject, motivationCard);
        }
    }
}
