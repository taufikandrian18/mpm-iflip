using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.LogActivity;
using System;
using System.Linq;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class PermissionsAppService : FLPAppServiceBase, IPermissionsAppService
    {
        private readonly IRepository<TBPermissions, Guid> _permissionRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;
        public PermissionsAppService(
            IRepository<TBPermissions, Guid> permissionRepository,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService)
        {
            _permissionRepository = permissionRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public IQueryable<TBPermissions> GetAll()
        {
            return _permissionRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername));
        }

        public TBPermissions GetById(Guid id)
        {
            return _permissionRepository.GetAll().FirstOrDefault(x => x.Id == id);
        }

        public void Create(TBPermissions input)
        {
            var motivationId = _permissionRepository.InsertAndGetId(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Permissions", motivationId, input.Code, LogAction.Create.ToString(), null, input);

        }

        public void Update(TBPermissions input)
        {
            var oldObject = _permissionRepository.GetAll().AsNoTracking().FirstOrDefault(x => x.Id == input.Id);
            _permissionRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Permissions", input.Id, input.Code, LogAction.Update.ToString(), oldObject, input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var permissions = _permissionRepository.FirstOrDefault(x => x.Id == id);
            var oldObject = _permissionRepository.GetAll().AsNoTracking().FirstOrDefault(x => x.Id == id);
            permissions.DeleterUsername = username;
            permissions.DeletionTime = DateTime.Now;
            _permissionRepository.Update(permissions);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "Permissions", id, permissions.Code, LogAction.Delete.ToString(), oldObject, permissions);
        }
    }
}
