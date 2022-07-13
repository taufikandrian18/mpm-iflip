using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System;
using MPM.FLP.Services.Dto;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MPM.FLP.Services.Backoffice
{
    public interface IProductCatalogController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        ProductCatalogs GetByIDBackoffice(Guid guid);
        List<ProductCategories> GetCategories();
        Task<ProductCatalogs> Create(ProductCatalogsVM model, IEnumerable<IFormFile> files, IEnumerable<IFormFile> tvcvideo1, IEnumerable<IFormFile> tvcvideo2, IEnumerable<IFormFile> tvcvideo3);
        Task<string> Import([FromForm]IEnumerable<IFormFile> excel);
        Task<ProductCatalogs> Edit(ProductCatalogsVM model, IEnumerable<IFormFile> files, IEnumerable<IFormFile> tvcvideo1, IEnumerable<IFormFile> tvcvideo2, IEnumerable<IFormFile> tvcvideo3);
        String Destroy(Guid guid);
        ProductCategories Spesifikasi(Guid id);
        BaseResponse VarianWarna(Pagination request, Guid id);
        Task<ProductColorVariants> CreateVarianWarna(ProductCatalogWarnaVM model, IEnumerable<IFormFile> files, string picker);
        Task<ProductColorVariants> EditVarianWarna(ProductCatalogWarnaEditVM model, IEnumerable<IFormFile> files, string picker);
        String DestroyWarna(Guid guid);
        BaseResponse FiturUtama(Pagination request, Guid id);
        Task<ProductFeatures> CreateFiturUtama(ProductCatalogFiturUtamaVM model, IEnumerable<IFormFile> files);
        Task<ProductFeatures> EditFiturUtama(ProductCatalogFiturUtamaEditVM model, IEnumerable<IFormFile> files);
        String DestroyFiturUtama(Guid guid);
        BaseResponse Aksesoris(Pagination request, Guid id);
        Task<ProductAccesories> CreateAksesoris(ProductCatalogAksesorisEditVM model, IEnumerable<IFormFile> files);
        Task<ProductAccesories> EditAksesoris(ProductCatalogAksesorisEditVM model, IEnumerable<IFormFile> files);
        String DestroyAksesoris(Guid guid);
        ProductPrices VariantHarga(Guid id);
        ActionResult DownloadTemplate(Guid id);
        Task<ProductPrices> CreateVarianHarga(IEnumerable<IFormFile> files);
        String EditVarianHarga(ProductPrices productPrices);
    }
}