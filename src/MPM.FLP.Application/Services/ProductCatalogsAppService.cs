using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Authorization;
using MPM.FLP.Authorization.Users;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.LogActivity;
using MPM.FLP.Services.Backoffice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class ProductCatalogAppService : FLPAppServiceBase, IProductCatalogAppService
    {
        private readonly IRepository<ProductCatalogs, Guid> _productCatalogRepository;
        private readonly IRepository<ProductPrices, Guid> _productPricesRepository;
        private readonly IAbpSession _abpSession;
        private readonly IRepository<InternalUsers> _internalUserRepository;
        private readonly IRepository<ProductCatalogAttachments, Guid> _productCatalogAttachmentsRepository;
        private readonly LogActivityAppService _logActivityAppService;

        public ProductCatalogAppService(IRepository<ProductCatalogs, Guid> productCatalogRepository, 
                                        IAbpSession abpSession, 
                                        IRepository<InternalUsers> internalUserRepository,
                                        IRepository<ProductPrices, Guid> productPricesRepository,
                                        IRepository<ProductCatalogAttachments, Guid> productCatalogAttachmentsRepository,
                                        LogActivityAppService logActivityAppService)
        {
            _productCatalogRepository = productCatalogRepository;
            _internalUserRepository = internalUserRepository;
            _abpSession = abpSession;
            _productPricesRepository = productPricesRepository;
            _productCatalogAttachmentsRepository = productCatalogAttachmentsRepository;
            _logActivityAppService = logActivityAppService;
        }

        public IQueryable<ProductCatalogs> GetAll()
        {
            return _productCatalogRepository.GetAll().Where(x=> x.IsPublished == true && string.IsNullOrEmpty(x.DeleterUsername)).OrderBy(x=> x.Order).ThenBy(x=> x.Title).Include(x => x.ProductCatalogAttachments);
        }

        public IQueryable<ProductCatalogs> GetAllBackoffice()
        {
            return _productCatalogRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).OrderBy(x => x.Order).ThenBy(x => x.Title).Include(x => x.ProductCatalogAttachments);
        }

        public IQueryable<ProductCatalogs> GetAllAdmin()
        {
            return _productCatalogRepository.GetAll().OrderBy(x => x.Order).ThenBy(x => x.Title).Include(x => x.ProductCatalogAttachments);
        }


        //public ICollection<ProductCatalogAttachments> GetAllAttachments(Guid id)
        //{
        //    var productCatalogs = _productCatalogRepository.GetAll().Include(y => y.ProductCatalogAttachments);
        //    var attachments = productCatalogs.FirstOrDefault(x => x.Id == id).ProductCatalogAttachments;
        //    return attachments;

        //}

        public BaseResponse GetAllAttachments([FromQuery] Pagination request)
        {
            request = Paginate.Validate(request);
            var count = 0;
            //var query = _productCatalogAttachmentsRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername)).Include(x=> x.ProductCatalog).ToList();
            //if (!string.IsNullOrEmpty(request.Query))
            //{
            //    query =  query.Where(x=> x.ProductCatalog.Title.Contains(request.Query)).ToList();
            //} 

            //if (!string.IsNullOrEmpty(request.ParentId)){
            //    query = query.Where(x=> x.ProductCatalog.Id.ToString().Contains(request.ParentId)).ToList();
            //}

            var query = _productCatalogRepository.GetAll()
                        .Where(x => string.IsNullOrEmpty(x.DeleterUsername) && !string.IsNullOrEmpty(x.SparepartDocUrl))
                        .Select(x => new { x.Id, x.IsPublished, x.Title, x.SparepartDocUrl, x.DeletionTime, x.CreationTime })
                        .ToList();
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(x => x.Title.ToLower().Contains(request.Query.ToLower())).ToList();
            }

            query = query.Skip(request.Page).Take(request.Limit).OrderByDescending(x => x.CreationTime).ToList();
            count = query.Count();
            return BaseResponse.Ok(query, count);
        }
        public List<Guid> GetAllIds()
        {
            return _productCatalogRepository.GetAll().Where(x=> x.IsPublished).Select(x => x.Id).ToList();
        }


        public List<Guid> GetAllIdsByCategories(Guid categoryId)
        {
            var productCatalogs = _productCatalogRepository.GetAll()
                                                           .Where(x => x.ProductCategoryId == categoryId 
                                                                    && x.IsPublished 
                                                                    && string.IsNullOrEmpty(x.DeleterUsername));

            return productCatalogs.Select(x => x.Id).ToList();
        }

        public ProductCatalogs GetByIdAdmin(Guid id)
        {
            var productCatalogs = _productCatalogRepository.GetAll()
                                                           .Include(x => x.ProductCatalogAttachments)
                                                           .Include(x => x.ProductAccesories)
                                                           .Include(x => x.ProductColorVariants)
                                                           .Include(x => x.ProductFeatures)
                                                           .FirstOrDefault(x => x.Id == id);

            try
            {
                long currentUserId = _abpSession.UserId.Value;
                var interalUser = _internalUserRepository.GetAll().FirstOrDefault(x => x.AbpUserId.Value == currentUserId);
                if (interalUser != null) 
                {
                
                        var kodeDealer = interalUser.KodeDealerMPM;
                        var productPrices = _productPricesRepository.GetAll().Where(x => x.KodeDealerMPM == kodeDealer).ToList();
                        foreach (var colorVariant in productCatalogs.ProductColorVariants)
                        {
                            var productPrice = productPrices.Where(x => x.ProductColorVariantId == colorVariant.Id).FirstOrDefault();
                            if (productPrice != null)
                            {
                                colorVariant.Price = productPrice.Price;
                            }
                        }   
                }
            }
            catch (Exception) { }

            return productCatalogs;
        }

        public ProductCatalogs GetById(Guid id)
        {
            var productCatalogs = _productCatalogRepository.GetAll().Where(x => x.IsPublished)
                                                           .Include(x => x.ProductCatalogAttachments)
                                                           .Include(x => x.ProductAccesories)
                                                           .Include(x => x.ProductColorVariants)
                                                           .Include(x => x.ProductFeatures)
                                                           .FirstOrDefault(x => x.Id == id);

            try
            {
                long currentUserId = _abpSession.UserId.Value;
                var interalUser = _internalUserRepository.GetAll().FirstOrDefault(x => x.AbpUserId.Value == currentUserId);
                if (interalUser != null)
                {

                    var kodeDealer = interalUser.KodeDealerMPM;
                    var productPrices = _productPricesRepository.GetAll().Where(x => x.KodeDealerMPM == kodeDealer).ToList();
                    foreach (var colorVariant in productCatalogs.ProductColorVariants)
                    {
                        var productPrice = productPrices.Where(x => x.ProductColorVariantId == colorVariant.Id).FirstOrDefault();
                        if (productPrice != null)
                        {
                            colorVariant.Price = productPrice.Price;
                        }
                    }
                }
            }
            catch (Exception) { }

            return productCatalogs;
        }

        public void Create(ProductCatalogs input)
        {
            _productCatalogRepository.Insert(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Katalog Produk", input.Id, input.Title, LogAction.Create.ToString(), null, input);
        }

        public void Update(ProductCatalogs input)
        {
            var oldObject = _productCatalogRepository.GetAll()
                .AsNoTracking()
                .Include(x => x.ProductCatalogAttachments)
                .Include(x => x.ProductAccesories)
                .Include(x => x.ProductColorVariants)
                .Include(x => x.ProductFeatures)
                .FirstOrDefault(x => x.Id == input.Id);

            _productCatalogRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Katalog Produk", input.Id, input.Title, LogAction.Update.ToString(), oldObject, input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var oldObject = _productCatalogRepository.GetAll()
                .AsNoTracking()
                .Include(x => x.ProductCatalogAttachments)
                .Include(x => x.ProductAccesories)
                .Include(x => x.ProductColorVariants)
                .Include(x => x.ProductFeatures)
                .FirstOrDefault(x => x.Id == id);

            var productCatalog = _productCatalogRepository.FirstOrDefault(x => x.Id == id);
            productCatalog.DeleterUsername = username;
            productCatalog.DeletionTime = DateTime.Now;
            _productCatalogRepository.Update(productCatalog);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "Katalog Produk", id, productCatalog.Title, LogAction.Delete.ToString(), oldObject, productCatalog);
        }
    }
}
