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
    public class SalesIncentiveProgramTargetAppService : FLPAppServiceBase, ISalesIncentiveProgramTargetAppService
    {
        private readonly IRepository<SalesIncentiveProgramTarget, Guid> _repositorySalesTarget;
        public SalesIncentiveProgramTargetAppService(
            IRepository<SalesIncentiveProgramTarget, Guid> repositorySalesTarget)
        {
            _repositorySalesTarget = repositorySalesTarget;
        }

        public BaseResponse GetAll([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _repositorySalesTarget.GetAll().Where(x => x.DeletionTime == null);
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Kota.Contains(request.Query));
            }

            var count = query.Count();
            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        public SalesIncentiveProgramTarget GetById(Guid Id)
        {
            var data = _repositorySalesTarget.GetAll()
                        .FirstOrDefault(x => x.Id == Id);
            return data;
        }

        public void Create(SalesIncentiveProgramTargetCreateDto input)
        {
            #region Create Sales Incentive Program Target
            var target = ObjectMapper.Map<SalesIncentiveProgramTarget>(input);
            target.CreationTime = DateTime.Now;
            target.CreatorUsername = this.AbpSession.UserId.ToString();

            _repositorySalesTarget.Insert(target);
            #endregion
        }
        public void Update(SalesIncentiveProgramTargetUpdateDto input)
        {
            #region Update Sales Incentive Program Target
            var target = _repositorySalesTarget.Get(input.Id);
            target.Kota = input.Kota;
            target.DealerId = input.DealerId;
            target.DealerName = input.DealerName;
            target.Karesidenan = input.Karesidenan;
            target.EnumTipeTransaksi = input.EnumTipeTransaksi;
            target.Target = input.Target;
            target.Transaksi = input.Transaksi;
            target.Capaian = input.Capaian;
            target.LastModifierUsername = this.AbpSession.UserId.ToString();
            target.LastModificationTime = DateTime.Now;

            _repositorySalesTarget.Update(target);
            #endregion

        }
        public void SoftDelete(SalesIncentiveProgramTargetDeleteDto input)
        {
            var target = _repositorySalesTarget.Get(input.Id);
            target.DeleterUsername = this.AbpSession.UserId.ToString();
            target.DeletionTime = DateTime.Now;
            _repositorySalesTarget.Update(target);
        }
    }
}
