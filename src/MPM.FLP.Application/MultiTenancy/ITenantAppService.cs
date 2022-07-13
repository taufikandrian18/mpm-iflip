using Abp.Application.Services;
using Abp.Application.Services.Dto;
using MPM.FLP.MultiTenancy.Dto;

namespace MPM.FLP.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

