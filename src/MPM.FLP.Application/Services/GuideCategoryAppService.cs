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
    public class GuideCategoryAppService : FLPAppServiceBase, IGuideCategoryAppService
    {
        private readonly IRepository<GuideCategories, Guid> _guideCategoryRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;

        public GuideCategoryAppService(
            IRepository<GuideCategories, Guid> guideRepository,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService)
        {
            _guideCategoryRepository = guideRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public IQueryable<GuideCategories> GetAll()
        {
            return _guideCategoryRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername));
        }

        public List<Guid> GetAllIds()
        {
            return _guideCategoryRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderBy(x => x.Order).Select(x => x.Id).ToList();
        }

        public GuideCategories GetById(Guid id)
        {
            var guide = _guideCategoryRepository.GetAll().Include(x => x.Guides).FirstOrDefault(x => x.Id == id);

            return guide;
        }

        public void Create(GuideCategories input)
        {
            _guideCategoryRepository.Insert(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Kategori Panduan Layanan", input.Id, input.Name, LogAction.Create.ToString(), null, input);
        }

        public void Update(GuideCategories input)
        {
            var oldObject = _guideCategoryRepository.GetAll().AsNoTracking().Include(x => x.Guides).FirstOrDefault(x => x.Id == input.Id);
            _guideCategoryRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Kategori Panduan Layanan", input.Id, input.Name, LogAction.Update.ToString(), oldObject, input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var oldObject = _guideCategoryRepository.GetAll().AsNoTracking().Include(x => x.Guides).FirstOrDefault(x => x.Id == id);
            var guide = _guideCategoryRepository.FirstOrDefault(x => x.Id == id);
            guide.DeleterUsername = username;
            guide.DeletionTime = DateTime.Now;
            _guideCategoryRepository.Update(guide);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "Kategori Panduan Layanan", id, guide.Name, LogAction.Delete.ToString(), oldObject, guide);
        }
    }
}
