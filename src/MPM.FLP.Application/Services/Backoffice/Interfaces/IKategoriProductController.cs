using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;

namespace MPM.FLP.Services.Backoffice
{
    public interface IKategoriProductController : IApplicationService
    {
        BaseResponse GetAllBackoffice(Pagination request);
        ProductCategories GetByIDBackoffice(Guid guid);
        Task<ProductCategories> Create(ProductCategories model, IEnumerable<IFormFile> files);
        Task<ProductCategories> Edit(ProductCategories model, IEnumerable<IFormFile> files);
        String Destroy(Guid guid);
    }
}