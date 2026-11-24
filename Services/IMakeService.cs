using Challenge_1_Build_APIS.Controllers.Resources;

namespace Challenge_1_Build_APIS.Services
{
    public interface IMakeService
    {


        Task<ServiceResult<PagedResult<MakeResource>>> GetAllAsync(int page = 1, int pageSize = 5); Task<ServiceResult<MakeResource>> GetByIdAsync(int id);
        Task<ServiceResult<MakeResource>> CreateAsync(SaveMakeResource resource);
        Task<ServiceResult<MakeResource>> UpdateAsync(int id,SaveMakeResource resource);
        Task<ServiceResult<MakeResource>> DeleteAsync(int id);



    }
}
