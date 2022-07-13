using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface IProductCatalogAttachmentAppService : IApplicationService
    {
        ProductCatalogAttachments GetById(Guid id);
        void Create(ProductCatalogAttachments input);
        void Update(ProductCatalogAttachments input);
        void SoftDelete(Guid id, string username);
    }
}
