using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;

namespace MPM.FLP.Services.Backoffice
{
    public interface ISparepartCatalogController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        ProductCatalogs GetByIDBackoffice(Guid guid);
        Task<ProductCatalogs> UploadDocument(Guid id, string SparepartDocUrl);
        Task<ProductCatalogs> Edit(ProductCatalogs model, IEnumerable<IFormFile> files);
        String Destroy(Guid guid);
        IActionResult DownloadFile(Guid id);
    }
}