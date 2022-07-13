using Abp.Application.Services;
using Microsoft.AspNetCore.Http;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IRolePlayResultAppService : IApplicationService
    {
        IQueryable<RolePlayResults> GetAll();
        List<RolePlayResults> GetAllItemByUser(int idmpm);
        RolePlayResults GetById(Guid id);
        ServiceResult Create(RolePlayResultDto input);
        Task<UploadResult> Upload(IFormFile video);
        void Validate(RolePlayResults input);
        void SoftDelete(Guid id, string username);
    }
}
