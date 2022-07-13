using Abp.Domain.Repositories;
using CorePush.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public class FincoyAppService : FLPAppServiceBase, IFincoyAppService
    {
        private readonly IRepository<Fincoy, Guid> _repositoryFincoy;
        public FincoyAppService(
            IRepository<Fincoy, Guid> repositoryFincoy)
        {
            _repositoryFincoy = repositoryFincoy;
        }

        public BaseResponse GetAll([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _repositoryFincoy.GetAll().Where(x => x.DeletionTime == null);
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.FincoyName.Contains(request.Query));
            }

            var count = query.Count();
            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        public Fincoy GetById(Guid Id)
        {
            var data = _repositoryFincoy.GetAll()
                        .FirstOrDefault(x => x.Id == Id);
            return data;
        }

        public void Create(FincoyCreateDto input)
        {
            #region Create Fincoy
            var fincoy = ObjectMapper.Map<Fincoy>(input);
            fincoy.CreationTime = DateTime.Now;
            fincoy.CreatorUsername = this.AbpSession.UserId.ToString();

            _repositoryFincoy.Insert(fincoy);
            #endregion
        }
        public void Update(FincoyUpdateDto input)
        {
            #region Update Fincoy
            var fincoy = _repositoryFincoy.Get(input.Id);
            fincoy.FincoyName = input.FincoyName;
            fincoy.LastModifierUsername = this.AbpSession.UserId.ToString();
            fincoy.LastModificationTime = DateTime.Now;

            _repositoryFincoy.Update(fincoy);
            #endregion

        }
        public void SoftDelete(ProductTypesDeleteDto input)
        {
            var productType = _repositoryFincoy.Get(input.Id);
            productType.DeleterUsername = this.AbpSession.UserId.ToString();
            productType.DeletionTime = DateTime.Now;
            _repositoryFincoy.Update(productType);
        }
    }
}
