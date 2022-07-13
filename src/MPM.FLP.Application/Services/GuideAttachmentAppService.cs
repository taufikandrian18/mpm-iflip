using Abp.Authorization;
using Abp.Domain.Repositories;
using MPM.FLP.Azure;
using MPM.FLP.FLPDb;
using MPM.FLP.Pdf;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class GuideAttachmentAppService : FLPAppServiceBase, IGuideAttachmentAppService
    {
        private readonly IRepository<GuideAttachments, Guid> _guideAttachmentRepository;
        private readonly IRepository<InternalUsers, int> _internalUserRepository;
        private readonly IAzureStorage _azureStorage;

        public GuideAttachmentAppService(IRepository<GuideAttachments, Guid> guideAttachmentRepository, IRepository<InternalUsers, int> internalUserRepository, IAzureStorage azureStorage)
        {
            _guideAttachmentRepository = guideAttachmentRepository;
            _internalUserRepository = internalUserRepository;
            _azureStorage = azureStorage;
        }

        public GuideAttachments GetById(Guid id)
        {
            var guideAttachment = _guideAttachmentRepository.FirstOrDefault(x => x.Id == id);
            return guideAttachment;
        }

        public void Create(GuideAttachments input)
        {
            _guideAttachmentRepository.Insert(input);
        }

        public void Update(GuideAttachments input)
        {
            _guideAttachmentRepository.Update(input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var guideAttachment = _guideAttachmentRepository.FirstOrDefault(x => x.Id == id);
            guideAttachment.DeleterUsername = username;
            guideAttachment.DeletionTime = DateTime.Now;
            _guideAttachmentRepository.Update(guideAttachment);
        }

        public async Task<string> GetAttachmentDownloadUrl(Guid id)
        {
            var currentUser = await GetCurrentUserAsync();
            int.TryParse(currentUser.UserName, out int mpmId);
            if (mpmId == 0) throw new Exception("User not found");

            var internalUser = _internalUserRepository.Get(mpmId);

            GuideAttachments guide = _guideAttachmentRepository.Get(id);
            string filename = guide.Title;

            string attachmentUrl = guide.StorageUrl;
            bool isPdf = filename.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase);
            if (!isPdf) return attachmentUrl;

            byte[] content = await _azureStorage.GetBlobContentBytes("panduanteknikal", filename);
            content = PdfHelper.AddWatermark(content, string.Format("Dealer Id: {0}\nFLP Id: {1}", internalUser.KodeDealerMPM, mpmId));
            Stream stream = new MemoryStream(content);
            attachmentUrl = await _azureStorage.UploadTempBlobAndGetUrl(string.Format("{0}-{1}-{2}", internalUser.KodeDealerMPM, mpmId, guide.Title), stream, true);

            return attachmentUrl;
        }
    }
}
