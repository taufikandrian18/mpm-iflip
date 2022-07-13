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
    public class InfoMainDealerAppService : FLPAppServiceBase, IInfoMainDealerAppService
    {
        private readonly IRepository<InfoMainDealers, Guid> _infoMainDealerRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;

        public InfoMainDealerAppService(
            IRepository<InfoMainDealers, Guid> infoMainDealerRepository,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService)
        {
            _infoMainDealerRepository = infoMainDealerRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public IQueryable<InfoMainDealers> GetAll()
        {
            return _infoMainDealerRepository.GetAll().Include(y => y.InfoMainDealerAttachments);
        }


        public List<Guid> GetAllIds()
        {
            return _infoMainDealerRepository.GetAll().Where(x => x.IsPublished && string.IsNullOrEmpty(x.DeleterUsername))
                            .OrderByDescending(x => x.CreationTime).Select(x => x.Id).ToList();

        }

        public ICollection<InfoMainDealerAttachments> GetAllAttachments(Guid id)
        {
            var infoMainDealers = _infoMainDealerRepository.GetAll().Include(x => x.InfoMainDealerAttachments);
            var attachments = infoMainDealers.FirstOrDefault(x => x.Id == id).InfoMainDealerAttachments;
            return attachments;
        }

        public InfoMainDealers GetById(Guid id)
        {
            var infoMainDealer = _infoMainDealerRepository.GetAll().Include(x => x.InfoMainDealerAttachments).FirstOrDefault(x => x.Id == id);
            return infoMainDealer;
        }

        public void Create(InfoMainDealers input)
        {
            _infoMainDealerRepository.Insert(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Info Main Dealer", input.Id, input.Title, LogAction.Create.ToString(), null, input);
        }

        public void Update(InfoMainDealers input)
        {
            var oldObject = _infoMainDealerRepository.GetAll().AsNoTracking().Include(x => x.InfoMainDealerAttachments).FirstOrDefault(x => x.Id == input.Id);
            _infoMainDealerRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Info Main Dealer", input.Id, input.Title, LogAction.Update.ToString(), oldObject, input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var oldObject = _infoMainDealerRepository.GetAll().AsNoTracking().FirstOrDefault(x => x.Id == id);
            var infoMainDealer = _infoMainDealerRepository.FirstOrDefault(x => x.Id == id);
            if(infoMainDealer != null){
                infoMainDealer.DeleterUsername = username;
                infoMainDealer.DeletionTime = DateTime.Now;
                _infoMainDealerRepository.Update(infoMainDealer);
                _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "Info Main Dealer", id, infoMainDealer.Title, LogAction.Delete.ToString(), oldObject, infoMainDealer);
            }
        }
    }
}
