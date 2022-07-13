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
    public interface ISelfRecordingResultAppService : IApplicationService
    {
        IQueryable<SelfRecordingResults> GetAll();
        List<SelfRecordingResults> GetAllItemByUser(int idmpm);
        SelfRecordingResults GetById(Guid id);
        ServiceResult Create(SelfRecordingResultDto input);
        Task<UploadResult> Upload(IFormFile video);
        void Validate(SelfRecordingResults input);
        void SoftDelete(Guid id, string username);
    }
}
