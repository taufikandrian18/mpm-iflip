using Abp.Application.Services;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace MPM.FLP.Services
{
    public interface ICustomerScanningAppService : IApplicationService
    {
        CustomerScanningOptionDto GetCustomerScanningOption();
        List<CustomerScanningResultDto> ScanCustomer(CustomerScanningSubmitDto input);
    }
}
