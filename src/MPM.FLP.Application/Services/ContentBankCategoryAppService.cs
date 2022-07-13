using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.LogActivity;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPM.FLP.Services
{
    public class ContentBankCategoryAppService : FLPAppServiceBase, IContentBankCategoryAppService
    {
        private readonly IRepository<ContentBankCategories, Guid> _repository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;
        public ContentBankCategoryAppService(
            IRepository<ContentBankCategories, Guid> repository,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService)
        {
            _repository = repository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public BaseResponse GetAll([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _repository.GetAll().Where(x => x.DeletionTime == null);
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Name.Contains(request.Query));
            }
            var count = query.Count();

            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        public void Create(ContentBankCategoriesCreateDto input)
        {
            var category = ObjectMapper.Map<ContentBankCategories>(input);
            category.CreationTime = DateTime.Now;

            var categoryId = _repository.InsertAndGetId(category);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Content Bank Category", categoryId, input.Name, LogAction.Create.ToString(), null, category);

        }

        public void Update(ContentBankCategoriesUpdateDto input)
        {
            var category = _repository.Get(input.Id);
            var oldObject = _repository.Get(input.Id);
            
            category.Name = input.Name;
            category.Orders = input.Orders;
            category.AttachmentUrl = input.AttachmentUrl;
            category.LastModifierUsername = input.LastModifierUsername;
            category.LastModificationTime = DateTime.Now;

            _repository.Update(category);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Content Bank Category", input.Id, input.Name, LogAction.Update.ToString(), oldObject, category);

        }

        public void SoftDelete(ContentBankCategoriesDeleteDto input)
        {
            var contentBankCategory = _repository.Get(input.Id);
            var oldObject = _repository.Get(input.Id);
            contentBankCategory.DeleterUsername = input.DeleterUsername;
            contentBankCategory.DeletionTime = DateTime.Now;

            _repository.Update(contentBankCategory);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.DeleterUsername, "Content Bank Category", input.Id, contentBankCategory.Name, LogAction.Delete.ToString(), oldObject, contentBankCategory);

        }
    }
}
