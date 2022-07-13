using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Authorization;
using MPM.FLP.Authorization.Users;
using MPM.FLP.FLPDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class ProductCatalogAttachmentAppService : FLPAppServiceBase, IProductCatalogAttachmentAppService
    {
        private readonly IRepository<ProductCatalogAttachments, Guid> _productCatalogAttachmentRepository;

        public ProductCatalogAttachmentAppService(IRepository<ProductCatalogAttachments, Guid> productCatalogAttachmentRepository)
        {
            _productCatalogAttachmentRepository = productCatalogAttachmentRepository;
        }
       
        public ProductCatalogAttachments GetById(Guid id)
        {
            var productCatalogAttachment = _productCatalogAttachmentRepository.FirstOrDefault(x => x.Id == id);
            return productCatalogAttachment;
        }

        public void Create(ProductCatalogAttachments input)
        {
            _productCatalogAttachmentRepository.Insert(input);
        }

        public void Update(ProductCatalogAttachments input)
        {
            _productCatalogAttachmentRepository.Update(input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var productCatalogAttachment = _productCatalogAttachmentRepository.FirstOrDefault(x => x.Id == id);
            productCatalogAttachment.DeleterUsername = username;
            productCatalogAttachment.DeletionTime = DateTime.Now;
            _productCatalogAttachmentRepository.Update(productCatalogAttachment);
        }
    }
}
