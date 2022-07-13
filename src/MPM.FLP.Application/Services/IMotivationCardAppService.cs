using Abp.Application.Services;
using MPM.FLP.FLPDb;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface IMotivationCardAppService : IApplicationService
    {
        IQueryable<MotivationCards> GetAll();
        List<string> GetAllImages();
        List<MotivationCardDto> GetAllTitleImages();
        MotivationCards GetById(Guid id);
        void Create(MotivationCards input);
        void Update(MotivationCards input);
        void SoftDelete(Guid id, string username);
    }
}
