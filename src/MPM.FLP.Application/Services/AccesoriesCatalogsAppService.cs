using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Authorization;
using MPM.FLP.Authorization.Users;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class AccesoriesCatalogAppService : FLPAppServiceBase, IAccesoriesCatalogAppService
    {
        private readonly IRepository<AccesoriesCatalogs, Guid> _accesoriesCatalogRepository;

        public AccesoriesCatalogAppService(IRepository<AccesoriesCatalogs, Guid> accesoriesCatalogRepository)
        {
            _accesoriesCatalogRepository = accesoriesCatalogRepository;
        }

        public IQueryable<AccesoriesCatalogs> GetAll()
        {
            return _accesoriesCatalogRepository.GetAll().Where(x=> x.DeletionTime == null);
        }

        public List<Guid> GetAllIds()
        {
            return _accesoriesCatalogRepository.GetAll().Where(x => x.IsPublished).Select(x => x.Id).ToList();
        }

        public AccesoriesCatalogs GetById(Guid id)
        {
            var accesoriesCatalogs = _accesoriesCatalogRepository.FirstOrDefault(x => x.Id == id);

            return accesoriesCatalogs;
        }

        public void Create(AccesoriesCatalogs input)
        {
            _accesoriesCatalogRepository.Insert(input);
        }

        public void Update(AccesoriesCatalogs input)
        {
            _accesoriesCatalogRepository.Update(input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var accesoriesCatalog = _accesoriesCatalogRepository.FirstOrDefault(x => x.Id == id);
            accesoriesCatalog.DeleterUsername = username;
            accesoriesCatalog.DeletionTime = DateTime.Now;
            _accesoriesCatalogRepository.Update(accesoriesCatalog);
        }
    }
}
