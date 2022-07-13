using Abp.Authorization;
using Abp.Domain.Repositories;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class CSChampionClubAttachmentAppService : FLPAppServiceBase, ICSChampionClubAttachmentAppService
    {
        private readonly IRepository<CSChampionClubAttachments, Guid> _csChampionClubAttachmentRepository;

        public CSChampionClubAttachmentAppService(IRepository<CSChampionClubAttachments, Guid> csChampionClubAttachmentRepository)
        {
            _csChampionClubAttachmentRepository = csChampionClubAttachmentRepository;
        }

        public CSChampionClubAttachments GetById(Guid id)
        {
            var csChampionClubAttachment = _csChampionClubAttachmentRepository.FirstOrDefault(x => x.Id == id);
            return csChampionClubAttachment;
        }

        public void Create(CSChampionClubAttachments input)
        {
            _csChampionClubAttachmentRepository.Insert(input);
        }

        public void Update(CSChampionClubAttachments input)
        {
            _csChampionClubAttachmentRepository.Update(input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var csChampionClubAttachment = _csChampionClubAttachmentRepository.FirstOrDefault(x => x.Id == id);
            csChampionClubAttachment.DeleterUsername = username;
            csChampionClubAttachment.DeletionTime = DateTime.Now;
            _csChampionClubAttachmentRepository.Update(csChampionClubAttachment);
        }
    }
}
