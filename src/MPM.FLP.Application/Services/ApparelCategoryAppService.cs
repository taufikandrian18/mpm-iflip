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
    public class ApparelCategoryAppService : FLPAppServiceBase, IApparelCategoryAppService
    {
        private readonly IRepository<ApparelCategories, Guid> _apparelCategoryRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;

        public ApparelCategoryAppService(
            IRepository<ApparelCategories, Guid> apparelCategoryRepository,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService)
        {
            _apparelCategoryRepository = apparelCategoryRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public IQueryable<ApparelCategories> GetAll()
        {
            return _apparelCategoryRepository.GetAll().Where(x=> x.DeletionTime == null);
        }

        public List<ApparelCategories> GetAllApparelCategoryItems()
        {
            return _apparelCategoryRepository.GetAll().Where(x=> x.IsPublished == true ).Include(x => x.ApparelCatalogs).OrderBy(x=> x.Order).ThenBy(x=> x.Name).ToList();
        }

        public List<Guid> GetAllIds()
        {
            return _apparelCategoryRepository.GetAll().Select(x => x.Id).ToList();
        }

        public ApparelCategories GetById(Guid id)
        {
            var apparel = _apparelCategoryRepository.GetAll()
                                                    .Include(x => x.ApparelCatalogs)
                                                    .FirstOrDefault(x => x.Id == id);

            return apparel;
        }

        public void Create(ApparelCategories input)
        {
            _apparelCategoryRepository.Insert(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Kategori Katalog Apparel", input.Id, input.Name, LogAction.Create.ToString(), null, input);
        }

        public void Update(ApparelCategories input)
        {
            var oldObject = _apparelCategoryRepository.GetAll().AsNoTracking().Include(x => x.ApparelCatalogs).FirstOrDefault(x => x.Id == input.Id);
            _apparelCategoryRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Kategori Katalog Apparel", input.Id, input.Name, LogAction.Update.ToString(), oldObject, input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var oldObject = _apparelCategoryRepository.GetAll().AsNoTracking().Include(x => x.ApparelCatalogs).FirstOrDefault(x => x.Id == id);
            var apparel = _apparelCategoryRepository.FirstOrDefault(x => x.Id == id);
            apparel.DeleterUsername = username;
            apparel.DeletionTime = DateTime.Now;
            _apparelCategoryRepository.Update(apparel);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "Kategori Katalog Apparel", id, apparel.Name, LogAction.Delete.ToString(), oldObject, apparel);
        }
    }
}