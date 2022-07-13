using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IVerifikasiJabatanAppService : IApplicationService
    {
        IQueryable<VerifikasiJabatans> GetAll();
        VerifikasiJabatans GetByGroupJabatan(string idGroupJabatan);
        VerifikasiJabatans GetById(Guid id);
        void Create(VerifikasiJabatans input);
        void Update(VerifikasiJabatans input);
        void SoftDelete(Guid id, string username);
    }
}
