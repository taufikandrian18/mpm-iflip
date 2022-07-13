using Abp.Application.Services;
using MPM.FLP.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MPM.FLP.Services
{
    public interface ICustomerEngagementAppService : IApplicationService
    {
        CustomerEngagementOptionDto GetCustomerEngagementOption();
        Task<List<CustomerEngagementDataDto>> EngageCustomer(CustomerEngagementSubmitDto input);
    }
}
