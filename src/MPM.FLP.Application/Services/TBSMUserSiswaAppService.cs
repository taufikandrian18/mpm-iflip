using Abp.Domain.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPM.FLP.Services
{
    public class TBSMUserSiswaAppService : FLPAppServiceBase, ITBSMUserSiswaAppService
    {
        private readonly IRepository<TBSMUserSiswas, Guid> _repository;
        private readonly IMapper _mapper;

        public TBSMUserSiswaAppService(IRepository<TBSMUserSiswas, Guid> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public BaseResponse GetAll([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _repository.GetAll().Where(x => x.DeletionTime == null);
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Nama.Contains(request.Query) || x.NIS.Contains(request.Query));
            }
            var count = query.Count();

            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        public TBSMUserSiswas GetById(Guid id)
        {
            return _repository.FirstOrDefault(id);
        }

        public List<TBSMUserSiswas> GetBySekolah(Guid id)
        {
            return _repository.GetAllList(x => x.GUIDSekolah == id);
        }

        public List<TBSMUserSiswas> GetByNpsn(string NPSN)
        {
            return _repository.GetAllList(x => x.NPSN == NPSN);
        }

         public List<TBSMUserSiswas> GetByNISN(string NISN)
        {
            return _repository.GetAllList(x => x.NIS == NISN);
        }

        public void Create(TBSMUserSiswasCreateDto input)
        {
            var siswa = ObjectMapper.Map<TBSMUserSiswas>(input);
            siswa.CreationTime = DateTime.Now;

            _repository.Insert(siswa);
        }

        public void Update(TBSMUserSiswasUpdateDto input)
        {
            var siswa = _repository.FirstOrDefault(input.Id);
            siswa = _mapper.Map(input, siswa);
            siswa.LastModificationTime = DateTime.Now;

            _repository.Update(siswa);
        }
        
        public void SoftDelete(TBSMUserSiswasDeleteDto input)
        {
            var siswa = _repository.Get(input.Id);
            siswa.DeleterUsername = input.DeleterUsername;
            siswa.DeletionTime = DateTime.Now;

            _repository.Update(siswa);
        }
    }
}
