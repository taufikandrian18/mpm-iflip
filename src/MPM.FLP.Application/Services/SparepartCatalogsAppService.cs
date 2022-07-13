using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Authorization;
using MPM.FLP.Authorization.Users;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.LogActivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class SparepartCatalogAppService : FLPAppServiceBase, ISparepartCatalogAppService
    {
        private readonly IRepository<SparepartCatalogs, Guid> _sparepartCatalogRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;

        public SparepartCatalogAppService(
            IRepository<SparepartCatalogs, Guid> sparepartCatalogRepository,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService)
        {
            _sparepartCatalogRepository = sparepartCatalogRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public IQueryable<SparepartCatalogs> GetAll()
        {
            return _sparepartCatalogRepository.GetAll();
        }

        public List<Guid> GetAllIds()
        {
            return _sparepartCatalogRepository.GetAll().Where(x => x.IsPublished).Select(x => x.Id).ToList();
        }

        public SparepartCatalogs GetById(Guid id)
        {
            var sparepartCatalogs = _sparepartCatalogRepository.FirstOrDefault(x => x.Id == id);

            return sparepartCatalogs;
        }

        public void Create(SparepartCatalogs input)
        {
            _sparepartCatalogRepository.Insert(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Katalog Spare Part", input.Id, input.Title, LogAction.Create.ToString(), null, input);
        }

        public void Update(SparepartCatalogs input)
        {
            var oldObject = _sparepartCatalogRepository.GetAll().AsNoTracking().FirstOrDefault(x => x.Id == input.Id);
            _sparepartCatalogRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Katalog Spare Part", input.Id, input.Title, LogAction.Update.ToString(), oldObject, input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var oldObject = _sparepartCatalogRepository.GetAll().AsNoTracking().FirstOrDefault(x => x.Id == id);
            var sparepartCatalog = _sparepartCatalogRepository.FirstOrDefault(x => x.Id == id);
            sparepartCatalog.DeleterUsername = username;
            sparepartCatalog.DeletionTime = DateTime.Now;
            _sparepartCatalogRepository.Update(sparepartCatalog);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "Katalog Spare Part", id, sparepartCatalog.Title, LogAction.Delete.ToString(), oldObject, sparepartCatalog);
        }
    }
}
