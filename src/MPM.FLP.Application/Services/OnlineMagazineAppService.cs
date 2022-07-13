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
    public class OnlineMagazineAppService : FLPAppServiceBase, IOnlineMagazineAppService
    {
        private readonly IRepository<OnlineMagazines, Guid> _onlineMagazineRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;

        public OnlineMagazineAppService(
            IRepository<OnlineMagazines, Guid> onlineMagazineRepository,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService)
        {
            _onlineMagazineRepository = onlineMagazineRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public IQueryable<OnlineMagazines> GetAll()
        {
            return _onlineMagazineRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername));
        }

        public List<Guid> GetAllIds()
        {
            return _onlineMagazineRepository.GetAll().Where(x => x.IsPublished && string.IsNullOrEmpty(x.DeleterUsername))
                            .OrderBy(x => x.Order).Select(x => x.Id).ToList();
            
        }

        public OnlineMagazines GetById(Guid id)
        {
            var onlineMagazine = _onlineMagazineRepository.GetAll().FirstOrDefault(x => x.Id == id);
            return onlineMagazine;
        }

        public void Create(OnlineMagazines input)
        {
            _onlineMagazineRepository.Insert(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Online Magazine", input.Id, input.Title, LogAction.Create.ToString(), null, input);
        }

        public void Update(OnlineMagazines input)
        {
            var oldObject = _onlineMagazineRepository.GetAll().AsNoTracking().FirstOrDefault(x => x.Id == input.Id);
            _onlineMagazineRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Online Magazine", input.Id, input.Title, LogAction.Update.ToString(), oldObject, input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var oldObject = _onlineMagazineRepository.GetAll().AsNoTracking().FirstOrDefault(x => x.Id == id);
            var onlineMagazine = _onlineMagazineRepository.FirstOrDefault(x => x.Id == id);
            onlineMagazine.DeleterUsername = username;
            onlineMagazine.DeletionTime = DateTime.Now;
            _onlineMagazineRepository.Update(onlineMagazine);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "Online Magazine", id, onlineMagazine.Title, LogAction.Delete.ToString(), oldObject, onlineMagazine);
        }
    }
}
