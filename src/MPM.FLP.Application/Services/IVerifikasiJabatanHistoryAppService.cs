using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IVerifikasiJabatanHistoryAppService
    {
        IQueryable<VerifikasiJabatanHistories> GetAll();
        IQueryable<VerifikasiJabatanHistories> GetByJabatan(int idJabatan);
        
        void Create(VerifikasiJabatanHistories input);
        VerifikasiJabatanHistoryDto GetByIDMPM(int idmpm);
        void Verify(Guid id);

        
    }
}
