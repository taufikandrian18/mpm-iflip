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
    public class VerifikasiJabatanAppService : FLPAppServiceBase, IVerifikasiJabatanAppService
    {
        private readonly IRepository<VerifikasiJabatans, Guid> _verifikasiJabatanRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;

        public VerifikasiJabatanAppService(
            IRepository<VerifikasiJabatans, Guid> verifikasiJabatanRepository,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService)
        {
            _verifikasiJabatanRepository = verifikasiJabatanRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public void Create(VerifikasiJabatans input)
        {
            //_verifikasiJabatanRepository.Insert(input);
            var insertId = _verifikasiJabatanRepository.InsertAndGetId(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Verifikasi Jabatan", insertId, input.Title, LogAction.Create.ToString(), null, input);
        }

        public IQueryable<VerifikasiJabatans> GetAll()
        {
            return _verifikasiJabatanRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername));
        }

        public VerifikasiJabatans GetById(Guid id)
        {
            return _verifikasiJabatanRepository.GetAll().FirstOrDefault(x => x.Id == id);
        }

        public VerifikasiJabatans GetByGroupJabatan(string idGroupJabatan)
        {
            return _verifikasiJabatanRepository.GetAll().Include(x => x.VerifikasiJabatanQuestions)
                .Where(x => string.IsNullOrEmpty(x.DeleterUsername)).FirstOrDefault(x => x.IDGroupJabatan == idGroupJabatan);
        }

        public void SoftDelete(Guid id, string username)
        {
            var verifikasiJabatan = _verifikasiJabatanRepository.FirstOrDefault(x => x.Id == id);
            var oldObject = _verifikasiJabatanRepository.GetAll().AsNoTracking().FirstOrDefault(x => x.Id == id);
            verifikasiJabatan.DeleterUsername = username;
            verifikasiJabatan.DeletionTime = DateTime.UtcNow.AddHours(7);
            _verifikasiJabatanRepository.Update(verifikasiJabatan);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "Verifikasi Jabatan", id, oldObject.Title, LogAction.Delete.ToString(), oldObject, oldObject);
        }

        public void Update(VerifikasiJabatans input)
        {
            var oldObject = _verifikasiJabatanRepository.GetAll().AsNoTracking().FirstOrDefault(x => x.Id == input.Id);
            _verifikasiJabatanRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Verifikasi Jabatan", input.Id, input.Title, LogAction.Update.ToString(), oldObject, input);

        }
    }
}
