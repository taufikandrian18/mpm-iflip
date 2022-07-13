using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Linq;

namespace MPM.FLP.Services
{
    public class SekolahAppService : FLPAppServiceBase, ISekolahAppService
    {
        private readonly IRepository<Sekolahs, Guid> _repository;

        public SekolahAppService(IRepository<Sekolahs, Guid> repository)
        {
            _repository = repository;
        }

        public BaseResponse GetAll([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _repository.GetAll().Where(x => x.DeletionTime == null);
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.NamaSMK.Contains(request.Query) ||
                                         x.NPSN.Contains(request.Query) ||
                                         x.Kota.Contains(request.Query) ||
                                         x.NamaMD.Contains(request.Query));
            }
            var count = query.Count();

            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        public Sekolahs GetById(Guid id)
        {
            return _repository.FirstOrDefault(id);
        }

        public void Create(Sekolahs input)
        {
            _repository.Insert(input);
        }

        public void Update(Sekolahs input)
        {
            _repository.Update(input);
        }
        
        public void SoftDelete(Guid id, string username)
        {
            var sekolah = _repository.Get(id);
            sekolah.DeleterUsername = username;
            sekolah.DeletionTime = DateTime.Now;

            _repository.Update(sekolah);
        }
    }
}
