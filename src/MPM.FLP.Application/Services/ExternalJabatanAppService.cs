using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Authorization;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.LogActivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public class ExternalJabatanAppService : FLPAppServiceBase, IExternalJabatanAppService
    {
        private readonly IRepository<ExternalJabatans, Guid> _externalJabatanRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;

        public ExternalJabatanAppService(
            IRepository<ExternalJabatans, Guid> externalJabatanRepository,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService)
        {
            _externalJabatanRepository = externalJabatanRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        [AbpAuthorize()]
        public IQueryable<ExternalJabatans> GetAll()
        {
            return _externalJabatanRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername));
        }

        public List<ExternalJabatans> GetList()
        {
            return _externalJabatanRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).ToList();
        }

        [AbpAuthorize()]
        public ExternalJabatans GetById(Guid id)
        {
            var externalJabatan = _externalJabatanRepository.GetAll().FirstOrDefault(x => x.Id == id);
            return externalJabatan;
        }

        [AbpAuthorize()]
        public void Create(ExternalJabatans input)
        {
            _externalJabatanRepository.Insert(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "External Jabatan", input.Id, input.Nama, LogAction.Create.ToString(), null, input);
        }

        [AbpAuthorize()]
        public void Update(ExternalJabatans input)
        {
            var oldObject = _externalJabatanRepository.GetAll().AsNoTracking().FirstOrDefault(x => x.Id == input.Id);
            _externalJabatanRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "External Jabatan", input.Id, input.Nama, LogAction.Update.ToString(), oldObject, input);
        }

        [AbpAuthorize()]
        public void SoftDelete(Guid id, string username)
        {
            var oldObject = _externalJabatanRepository.GetAll().AsNoTracking().FirstOrDefault(x => x.Id == id);
            var ExternalJabatan = _externalJabatanRepository.FirstOrDefault(x => x.Id == id);
            ExternalJabatan.DeleterUsername = username;
            ExternalJabatan.DeletionTime = DateTime.Now;
            _externalJabatanRepository.Update(ExternalJabatan);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "External Jabatan", id, ExternalJabatan.Nama, LogAction.Delete.ToString(), oldObject, ExternalJabatan);
        }
    }
}
