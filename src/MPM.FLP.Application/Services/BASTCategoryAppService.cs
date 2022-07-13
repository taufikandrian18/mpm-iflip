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
    public class BASTCategoryAppService : FLPAppServiceBase, IBASTCategoryAppService
    {
        private readonly IRepository<BASTCategories, Guid> _repositoryCategory;
        public BASTCategoryAppService(
            IRepository<BASTCategories, Guid> repositoryCategory)
        {
            _repositoryCategory = repositoryCategory; 
        }

        public BaseResponse GetAll([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _repositoryCategory.GetAll().Where(x => x.DeletionTime == null);
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Name.Contains(request.Query));
            }

            var count = query.Count();
            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        public BASTCategories GetById(Guid Id)
        {
            var data = _repositoryCategory.GetAll()
                        .FirstOrDefault(x => x.Id == Id);
            return data;
        }

        public void Create(BASTCategoryCreateDto input)
        {
            var category = ObjectMapper.Map<BASTCategories>(input);
            category.CreationTime = DateTime.Now;
            category.CreatorUsername = this.AbpSession.UserId.ToString();

            _repositoryCategory.Insert(category);
           
        }
        public void Update(BASTCategoryUpdateDto input)
        {
            var category = _repositoryCategory.Get(input.Id);
            category.Name = input.Name;
            category.LastModifierUsername = this.AbpSession.UserId.ToString();
            category.LastModificationTime = DateTime.Now;

            _repositoryCategory.Update(category);
        }
        public void SoftDelete(BASTCategoryDeleteDto input)
        {
            var category = _repositoryCategory.Get(input.Id);
            category.DeleterUsername = this.AbpSession.UserId.ToString();
            category.DeletionTime = DateTime.Now;
            _repositoryCategory.Update(category);
        }
    }
}
