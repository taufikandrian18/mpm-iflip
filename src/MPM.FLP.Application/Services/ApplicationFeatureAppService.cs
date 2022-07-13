using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.LogActivity;
using MPM.FLP.Services.Backoffice;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public class ApplicationFeatureAppService : FLPAppServiceBase, IApplicationFeatureAppService
    {
        private readonly IRepository<ApplicationFeature, Guid> _repositoryApplicationFeature;
        private readonly IRepository<ApplicationFeatureMapping, Guid> _repositoryMapping;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;
        public ApplicationFeatureAppService(
            IRepository<ApplicationFeature, Guid> repositoryApplicationFeature,
            IRepository<ApplicationFeatureMapping, Guid> repositoryMapping,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService)
        {
            _repositoryApplicationFeature = repositoryApplicationFeature;
            _repositoryMapping = repositoryMapping;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public BaseResponse GetAllAdmin([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = _repositoryApplicationFeature.GetAll().Where(x => x.DeletionTime == null);
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.MenuName.Contains(request.Query));
            }

            var count = query.Count();
            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        public BaseResponse GetUser([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);

            var query = (from menu in _repositoryApplicationFeature.GetAll().Where(x => x.DeletionTime == null)
                         join mapping in _repositoryMapping.GetAll().Where(x => x.DeletionTime == null)
                         on menu.Id equals mapping.GUIDFeature
                         select new ApplicationFeatureResponse
                         {
                             Id = mapping.Id,
                             IconUrl = menu.IconUrl,
                             MenuName = menu.MenuName,
                             EnumChannel = mapping.EnumChannel,
                             Status = mapping.Status
                         });

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.MenuName.Contains(request.Query));
            }

            if (request.IsH1 != null)
            {
                query = query.Where(x => x.EnumChannel == "H1").Where(x=> x.Status == 1);
            }

            if (request.IsH2 != null)
            {
                query = query.Where(x => x.EnumChannel == "H2").Where(x=> x.Status == 1);
            }

            if (request.IsH3 != null)
            {
                query = query.Where(x => x.EnumChannel == "H3").Where(x=> x.Status == 1);
            }

            if (request.IsTBSM != null)
            {
                query = query.Where(x => x.EnumChannel == "TBSM").Where(x=> x.Status == 1);
            }

            var count = query.Count();
            var data = query.Skip(request.Page).Take(request.Limit).ToList();

            return BaseResponse.Ok(data, count);
        }

        public ApplicationFeature GetById(Guid Id)
        {
            var data  = _repositoryApplicationFeature.GetAll()
                .Include(x => x.ApplicationFeatureMapping)
                .FirstOrDefault(x => x.Id == Id);
            data.ApplicationFeatureMapping = data.ApplicationFeatureMapping.OrderBy(x=> x.EnumChannel).ToList();
            return data;
        }
        public void Create(ApplicationFeatureCreateDto input)
        {
            #region Create Application Feature
            var applicationFeature = ObjectMapper.Map<ApplicationFeature>(input);
            applicationFeature.CreationTime = DateTime.Now;
            applicationFeature.CreatorUsername = this.AbpSession.UserId.ToString();

            Guid applicationFeatureId = _repositoryApplicationFeature.InsertAndGetId(applicationFeature);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, "admin", "Menu", applicationFeatureId, input.MenuName, LogAction.Create.ToString(), null, applicationFeature);
            #endregion

            #region Create Application Feature Mapping
            foreach (var mapping in input.Mapping)
            {
                var _mapping = ObjectMapper.Map<ApplicationFeatureMapping>(mapping);
                _mapping.GUIDFeature = applicationFeatureId;
                _mapping.CreatorUsername = this.AbpSession.UserId.ToString();
                _mapping.CreationTime = DateTime.Now;
                _repositoryMapping.Insert(_mapping);
            }
            #endregion
        }

        public void Update(ApplicationFeatureUpdateDto input)
        {
            #region Update Application Feature
            var applicationFeature = _repositoryApplicationFeature.Get(input.Id);
            var oldObject = _repositoryApplicationFeature.Get(input.Id);
            applicationFeature.IconUrl = input.IconUrl;
            applicationFeature.MenuName = input.MenuName;
            applicationFeature.LastModifierUsername = this.AbpSession.UserId.ToString();
            applicationFeature.LastModificationTime = DateTime.Now;

            _repositoryApplicationFeature.Update(applicationFeature);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, this.AbpSession.UserId.ToString(), "Menu", input.Id, input.MenuName, LogAction.Update.ToString(), oldObject, applicationFeature);
            #endregion

            #region Update application Feature Mapping
            var mappings = _repositoryMapping.GetAllList(x => x.GUIDFeature == input.Id && x.DeletionTime == null);
            foreach (var mapping in mappings)
            {
                var mapped = _repositoryMapping.GetAllList(x => x.Id == mapping.Id).FirstOrDefault();
                if (mapped != null)
                    _repositoryMapping.Delete(mapped);
            }

            foreach (var mapping in input.Mapping)
            {
                var _mapping = ObjectMapper.Map<ApplicationFeatureMapping>(mapping);
                _mapping.GUIDFeature = input.Id;
                _mapping.CreatorUsername = this.AbpSession.UserId.ToString();
                _mapping.CreationTime = DateTime.Now;
                _mapping.LastModifierUsername = this.AbpSession.UserId.ToString();
                _mapping.LastModificationTime = DateTime.Now;
                _repositoryMapping.Insert(_mapping);
            }
            #endregion

        }

        public void UpdateMapping(ApplicationFeatureMappingUpdateDto input)
        {
            #region Update Application Feature
            var mapping = _repositoryMapping.Get(input.Id);
            mapping.EnumChannel = input.EnumChannel;
            mapping.GUIDFeature = input.GUIDFeature;
            mapping.Status = input.Status;
            mapping.LastModifierUsername = this.AbpSession.UserId.ToString();
            mapping.LastModificationTime = DateTime.Now;

            _repositoryMapping.Update(mapping);
            #endregion

        }
        public void SoftDelete(ApplicationFeatureDeleteDto input)
        {
            var applicationFeature = _repositoryApplicationFeature.Get(input.Id);
            var oldObject = _repositoryApplicationFeature.Get(input.Id);
            applicationFeature.DeleterUsername = this.AbpSession.UserId.ToString();
            applicationFeature.DeletionTime = DateTime.Now;
            _repositoryApplicationFeature.Update(applicationFeature);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, this.AbpSession.UserId.ToString(), "Menu", input.Id, applicationFeature.MenuName, LogAction.Delete.ToString(), oldObject, applicationFeature);

            SoftDeleteMapping(input.Id);
        }

        private void SoftDeleteMapping(Guid FeatureId)
        {
            var mappings = _repositoryMapping.GetAllList(x => x.GUIDFeature == FeatureId && x.DeletionTime == null);
            foreach (var mapping in mappings)
            {
                mapping.DeleterUsername = this.AbpSession.UserId.ToString();
                mapping.DeletionTime = DateTime.Now;
                _repositoryMapping.Update(mapping);
            }
        }
    }
}
