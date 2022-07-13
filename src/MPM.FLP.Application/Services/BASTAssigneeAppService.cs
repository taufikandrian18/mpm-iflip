using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class BASTAssigneeAppService : FLPAppServiceBase, IBASTAssigneeAppService
    {
        private readonly IRepository<BASTAssignee, Guid> _BASTAssigneeRepository;

        public BASTAssigneeAppService(IRepository<BASTAssignee, Guid> BASTAssigneeRepository)
        {
            _BASTAssigneeRepository = BASTAssigneeRepository;
        }

        public BaseResponse GetAll([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _BASTAssigneeRepository.GetAll().Where(x => x.DeletionTime == null);
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Channel.Contains(request.Query));
            }

            var count = query.Count();
            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }
        public BASTAssignee GetById(Guid id)
        {
            var assignee = _BASTAssigneeRepository.FirstOrDefault(x => x.Id == id);
            return assignee;
        }

        public void Create(BASTAssigneeCreateDto input)
        {
            var assignee = ObjectMapper.Map<BASTAssignee>(input);
            assignee.CreationTime = DateTime.Now;
            assignee.CreatorUsername = this.AbpSession.UserId.ToString();
            _BASTAssigneeRepository.Insert(assignee);
        }

        public void Update(BASTAssigneeUpdateDto input)
        {
            var assignee = _BASTAssigneeRepository.Get(input.Id);
            assignee.BASTsId = input.GUIDBAST;
            assignee.GUIDEmployee = input.GUIDEmployee;
            assignee.Jabatan = input.Jabatan;
            assignee.DealerName = input.DealerName;
            assignee.Channel = input.Channel;
            assignee.KodeJaringan = input.KodeJaringan;
            assignee.TipeJaringan = input.TipeJaringan;
            assignee.Kota = input.Kota;
            assignee.LastModifierUsername = this.AbpSession.UserId.ToString();
            assignee.LastModificationTime = DateTime.Now;

            _BASTAssigneeRepository.Update(assignee);
        }

        public void SoftDelete(Guid id)
        {
            var assignee = _BASTAssigneeRepository.FirstOrDefault(x => x.Id == id);
            assignee.DeleterUsername = this.AbpSession.UserId.ToString();
            assignee.DeletionTime = DateTime.Now;
            _BASTAssigneeRepository.Update(assignee);
        }
    }
}
