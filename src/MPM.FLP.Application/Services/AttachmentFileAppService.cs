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
    public class AttachmentFileAppService : FLPAppServiceBase, IAttachmentFileAppService
    {
        private readonly IRepository<InternalUsers, int> _internalUserRepository;
        private readonly IAzureStorage _azureStorage;

        public AttachmentFileAppService(IRepository<InternalUsers, int> internalUserRepository, IAzureStorage azureStorage)
        {
            _internalUserRepository = internalUserRepository;
            _azureStorage = azureStorage;
        }

        public async Task<string> GetAttachmentDownloadUrl(string url)
        {
            if (!url.ToLower().EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase)) return url;

            var currentUser = await GetCurrentUserAsync();
            int.TryParse(currentUser.UserName, out int mpmId);
            if (mpmId == 0) throw new Exception("User not found");

            var internalUser = _internalUserRepository.Get(mpmId);

            string[] pathParts = url.Split('/');
            string filename = pathParts.Last(x => x.Contains(".pdf", StringComparison.InvariantCultureIgnoreCase));
            WebClient webClient = new WebClient();
            byte[] content = webClient.DownloadData(new Uri(url));

            content = PdfHelper.AddWatermark(content, string.Format("Dealer Id: {0}\nFLP Id: {1}", internalUser.KodeDealerMPM, mpmId));
            Stream stream = new MemoryStream(content);
            string attachmentUrl = await _azureStorage.UploadTempBlobAndGetUrl(string.Format("{0}-{1}-{2}", internalUser.KodeDealerMPM, mpmId, filename), stream, true);

            return attachmentUrl;
        }
    }
}
