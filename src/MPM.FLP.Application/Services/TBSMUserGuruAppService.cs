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
    public class TBSMUserGuruAppService : FLPAppServiceBase, ITBSMUserGuruAppService
    {
        private readonly IRepository<TBSMUserGurus, Guid> _repository;
        private readonly IMapper _mapper;

        public TBSMUserGuruAppService(IRepository<TBSMUserGurus, Guid> repository, IMapper mapper)
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
                query = query.Where(x => x.Nama.Contains(request.Query) || x.NIP.Contains(request.Query));
            }
            var count = query.Count();

            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        public TBSMUserGurus GetById(Guid id)
        {
            return _repository.FirstOrDefault(id);
        }

        public List<TBSMUserGurus> GetBySekolahId(Guid id)
        {
            return _repository.GetAllList(x => x.GUIDSekolah == id);
        }

        public List<TBSMUserGurus> GetByNpsn(string NPSN)
        {
            return _repository.GetAllList(x => x.NPSN == NPSN);
        }

        public List<TBSMUserGurus> GetByNIP(string NIP)
        {
            return _repository.GetAllList(x => x.NIP == NIP);
        }

        public void Create(TBSMUserGurusCreateDto input)
        {
            var guru = ObjectMapper.Map<TBSMUserGurus>(input);
            guru.CreationTime = DateTime.Now;

            _repository.Insert(guru);
        }

        public void Update(TBSMUserGurusUpdateDto input)
        {
            var guru = _repository.FirstOrDefault(input.Id);
            guru = _mapper.Map(input, guru);
            guru.LastModificationTime = DateTime.Now;

            _repository.Update(guru);
        }
        
        public void SoftDelete(TBSMUserGurusDeleteDto input)
        {
            var guru = _repository.Get(input.Id);
            guru.DeleterUsername = input.DeleterUsername;
            guru.DeletionTime = DateTime.Now;

            _repository.Update(guru);
        }
    }
}
