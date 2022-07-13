using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM.FLP.Services
{
    public interface ICSChampionClubAttachmentAppService : IApplicationService
    {
        CSChampionClubAttachments GetById(Guid id);
        void Create(CSChampionClubAttachments input);
        void Update(CSChampionClubAttachments input);
        void SoftDelete(Guid id, string username);
    }
}
