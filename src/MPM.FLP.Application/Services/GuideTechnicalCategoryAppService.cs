using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Authorization;
using MPM.FLP.Authorization.Users;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.LogActivity;
using MPM.FLP.Services.Backoffice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class GuideTechnicalCategoryAppService : FLPAppServiceBase, IGuideTechnicalCategoryAppService
    {
        private readonly IRepository<GuideTechnicalCategories, Guid> _guideTechnicalCategoryRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;

        public GuideTechnicalCategoryAppService(
            IRepository<GuideTechnicalCategories, Guid> guideTechnicalRepository,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService)
        {
            _guideTechnicalCategoryRepository = guideTechnicalRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public BaseResponse GetAllAdmin([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _guideTechnicalCategoryRepository.GetAll().Where(x => x.DeletionTime == null);
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Name.Contains(request.Query));
            }

            var count = query.Count();
            var data = query.Skip(request.Page).Take(request.Limit).OrderByDescending(x => x.CreationTime).ToList();

            return BaseResponse.Ok(data, count);
        }
        public List<Guid> GetAllIds()
        {
            return _guideTechnicalCategoryRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderBy(x => x.Order).Select(x => x.Id).ToList();
        }

        public GuideTechnicalCategories GetById(Guid id)
        {
            var guide = _guideTechnicalCategoryRepository.GetAll().Include(x => x.Guides).FirstOrDefault(x => x.Id == id);

            return guide;
        }

        public void Create(GuideTechnicalCategories input)
        {
            _guideTechnicalCategoryRepository.Insert(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Kategori Panduan Layanan", input.Id, input.Name, LogAction.Create.ToString(), null, input);
        }

        public void Update(GuideTechnicalCategories input)
        {
            var oldObject = _guideTechnicalCategoryRepository.GetAll().AsNoTracking().Include(x => x.Guides).FirstOrDefault(x => x.Id == input.Id);
            _guideTechnicalCategoryRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Kategori Panduan Layanan", input.Id, input.Name, LogAction.Update.ToString(), oldObject, input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var oldObject = _guideTechnicalCategoryRepository.GetAll().AsNoTracking().Include(x => x.Guides).FirstOrDefault(x => x.Id == id);
            var guide = _guideTechnicalCategoryRepository.FirstOrDefault(x => x.Id == id);
            guide.DeleterUsername = username;
            guide.DeletionTime = DateTime.Now;
            _guideTechnicalCategoryRepository.Update(guide);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "Kategori Panduan Teknikal", id, guide.Name, LogAction.Delete.ToString(), oldObject, guide);
        }
    }
}
