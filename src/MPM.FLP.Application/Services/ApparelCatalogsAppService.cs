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
    public class ApparelCatalogAppService : FLPAppServiceBase, IApparelCatalogAppService
    {
        private readonly IRepository<ApparelCatalogs, Guid> _apparelCatalogRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;

        public ApparelCatalogAppService(
            IRepository<ApparelCatalogs, Guid> apparelCatalogRepository,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService)
        {
            _apparelCatalogRepository = apparelCatalogRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public IQueryable<ApparelCatalogs> GetAll()
        {
            return _apparelCatalogRepository.GetAll().Where(x=> x.DeletionTime == null).Where(x=> x.IsPublished ==true).OrderBy(x=> x.Order).ThenBy(x=> x.Title);
        }

        public IQueryable<ApparelCatalogs> GetAllAdmin()
        {
            return _apparelCatalogRepository.GetAll().Where(x=> x.DeletionTime == null).OrderBy(x => x.Order).ThenBy(x => x.Title);
        }

        public List<Guid> GetAllIds()
        {
            return _apparelCatalogRepository.GetAll().Where(x => x.IsPublished).OrderBy(x=> x.Order).Select(x => x.Id).ToList();
        }

        public List<Guid> GetAllIdsByCategories(Guid categoryId)
        {
            var apparelCatalogs = _apparelCatalogRepository.GetAll()
                                                           .Where(x => x.ApparelCategoryId == categoryId
                                                                    && x.IsPublished
                                                                    && string.IsNullOrEmpty(x.DeleterUsername)).OrderBy(x=> x.Order);

            return apparelCatalogs.Select(x => x.Id).ToList();
        }


        public ApparelCatalogs GetById(Guid id)
        {
            var apparelCatalogs = _apparelCatalogRepository.FirstOrDefault(x => x.Id == id);

            return apparelCatalogs;
        }

        public void Create(ApparelCatalogs input)
        {
            _apparelCatalogRepository.Insert(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Katalog Apparel", input.Id, input.Title, LogAction.Create.ToString(), null, input);
        }

        public void Update(ApparelCatalogs input)
        {
            var oldObject = _apparelCatalogRepository.GetAll().AsNoTracking().FirstOrDefault(x => x.Id == input.Id);
            _apparelCatalogRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Katalog Apparel", input.Id, input.Title, LogAction.Update.ToString(), oldObject, input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var oldObject = _apparelCatalogRepository.GetAll().AsNoTracking().FirstOrDefault(x => x.Id == id);
            var apparelCatalog = _apparelCatalogRepository.FirstOrDefault(x => x.Id == id);
            apparelCatalog.DeleterUsername = username;
            apparelCatalog.DeletionTime = DateTime.Now;
            _apparelCatalogRepository.Update(apparelCatalog);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "Katalog Apparel", id, apparelCatalog.Title, LogAction.Delete.ToString(), oldObject, apparelCatalog);
        }
    }
}
