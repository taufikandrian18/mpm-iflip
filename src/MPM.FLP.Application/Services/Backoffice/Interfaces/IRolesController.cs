using Abp.Application.Services;
using System.Threading.Tasks;

namespace MPM.FLP.Services.Backoffice
{
    public interface IRolesController : IApplicationService
    {
        Task<RoleListViewModel> Index();
    }
}