using Abp.Authorization;
using Abp.Domain.Repositories;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class VerifikasiJabatanHistoryAppService : FLPAppServiceBase, IVerifikasiJabatanHistoryAppService
    {
        private readonly IRepository<VerifikasiJabatanHistories, Guid> _verifikasiJabatanHistoryRepository;

        public VerifikasiJabatanHistoryAppService(IRepository<VerifikasiJabatanHistories, Guid> verifikasiJabatanHistoryRepository)
        {
            _verifikasiJabatanHistoryRepository = verifikasiJabatanHistoryRepository;
        }

        public void Create(VerifikasiJabatanHistories input)
        {
            _verifikasiJabatanHistoryRepository.Insert(input);
        }

        public IQueryable<VerifikasiJabatanHistories> GetAll()
        {
            return _verifikasiJabatanHistoryRepository.GetAll();
        }

        public VerifikasiJabatanHistoryDto GetByIDMPM(int idmpm)
        {
            var history = _verifikasiJabatanHistoryRepository.GetAll()
                .Where(x => !x.IsVerified.Value || !x.IsVerified.HasValue)
                .FirstOrDefault(x => x.IDMPM == idmpm);

            Guid? id = history != null ? history.Id : (Guid?)null;
            var idGroupJabatan = history?.IDGroupJabatan;

            return new VerifikasiJabatanHistoryDto() { Id = id, IdGroupJabatan = idGroupJabatan };
        }

        public IQueryable<VerifikasiJabatanHistories> GetByJabatan(int idJabatan)
        {
            return _verifikasiJabatanHistoryRepository.GetAll().Where(x => x.IDJabatan == idJabatan);
        }

        public void Verify(Guid id)
        {
            var verifikasiJabatanHistory = _verifikasiJabatanHistoryRepository.FirstOrDefault(x => x.Id == id);
            verifikasiJabatanHistory.IsVerified = true;
            verifikasiJabatanHistory.LastModifierUsername = "System";
            verifikasiJabatanHistory.LastModificationTime = DateTime.UtcNow.AddHours(7);
            _verifikasiJabatanHistoryRepository.Update(verifikasiJabatanHistory);
        }
    }
}
