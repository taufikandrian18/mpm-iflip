using Abp.Application.Services;
using MPM.FLP.FLPDb;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;

namespace MPM.FLP.Services.Backoffice
{
    public interface IAzureController : IApplicationService
    {
        IConfigurationRoot GetConnectionToAzure();
        Task<string> InsertAndGetUrlAzure(IFormFile file, string id, string nama, string container);
    }
}
