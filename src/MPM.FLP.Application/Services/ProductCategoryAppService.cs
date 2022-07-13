using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using MPM.FLP.Authorization;
using MPM.FLP.Authorization.Users;
using MPM.FLP.Common.Enums;
using MPM.FLP.FLPDb;
using MPM.FLP.LogActivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    [AbpAuthorize()]
    public class ProductCategoryAppService : FLPAppServiceBase, IProductCategoryAppService
    {
        private readonly IRepository<ProductCategories, Guid> _productCategoryRepository;
        private readonly IRepository<ProductCatalogs, Guid> _productCatalogsRepository;
        private readonly IAbpSession _abpSession;
        private readonly LogActivityAppService _logActivityAppService;

        public ProductCategoryAppService(
            IRepository<ProductCategories, Guid> productRepository,
            IRepository<ProductCatalogs, Guid> catalogRepository,
            IAbpSession abpSession,
            LogActivityAppService logActivityAppService)
        {
            _productCategoryRepository = productRepository;
            _productCatalogsRepository = catalogRepository;
            _abpSession = abpSession;
            _logActivityAppService = logActivityAppService;
        }

        public IQueryable<ProductCategories> GetAll()
        {
            return _productCategoryRepository.GetAll().Where(x => string.IsNullOrEmpty(x.DeleterUsername));
        }

        public List<ProductCategories> GetAllProductCategoryItems(string query)
        {
            var q= _productCategoryRepository.GetAll().Include(x => x.ProductCatalogs).Where(x=> x.IsPublished == true).ToList();
            if(!String.IsNullOrEmpty(query)){
                q = q.Where(x=> query.Contains(x.Name)).ToList();
            }
            var data  = q.OrderBy(x => x.Order).ThenBy(x=> x.Name).ToList();
            return data;
        }

         public List<ProductCatalogs> GetAllProductCatalogs(string query, string categoryId)
        {
            var q= _productCatalogsRepository.GetAll().Where(x=> x.IsPublished == true).ToList();
            if(!String.IsNullOrEmpty(query)){
                q = q.Where(x=> query.Contains(x.ProductCode) || query.Contains(x.Title)).ToList();
            }

            if(!String.IsNullOrEmpty(categoryId)){
                q = q.Where(x=> x.ProductCategoryId.Value.ToString() == categoryId).ToList();
            }
            var data  = q.OrderBy(x => x.Order).ThenBy(x=> x.Title).ToList();
            return data;
        }

        public List<Guid> GetAllIds()
        {
            return _productCategoryRepository.GetAll().Select(x => x.Id).ToList();
        }

        public ProductCategories GetById(Guid id)
        {
            var product = _productCategoryRepository.GetAll()
                                                    .Include(x => x.ProductCatalogs)
                                                    .FirstOrDefault(x => x.Id == id);

            return product;
        }

        public void Create(ProductCategories input)
        {
            _productCategoryRepository.Insert(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.CreatorUsername, "Kategori Katalog Produk", input.Id, input.Name, LogAction.Create.ToString(), null, input);
        }

        public void Update(ProductCategories input)
        {
            var oldObject = _productCategoryRepository.GetAll().AsNoTracking().Include(x => x.ProductCatalogs).FirstOrDefault(x => x.Id == input.Id);
            _productCategoryRepository.Update(input);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, input.LastModifierUsername, "Kategori Katalog Produk", input.Id, input.Name, LogAction.Update.ToString(), oldObject, input);
        }

        public void SoftDelete(Guid id, string username)
        {
            var oldObject = _productCategoryRepository.GetAll().AsNoTracking().Include(x => x.ProductCatalogs).FirstOrDefault(x => x.Id == id);
            var product = _productCategoryRepository.FirstOrDefault(x => x.Id == id);
            product.DeleterUsername = username;
            product.DeletionTime = DateTime.Now;
            _productCategoryRepository.Update(product);
            _logActivityAppService.CreateLogActivity(_abpSession.UserId.Value, username, "Kategori Katalog Produk", id, product.Name, LogAction.Delete.ToString(), oldObject, product);
        }
    }
}
