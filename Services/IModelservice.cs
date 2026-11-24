using Challenge_1_Build_APIS.Controllers.Resources;

namespace Challenge_1_Build_APIS.Services
{
    public interface IModelService
    {
        Task<ServiceResult<PagedResult<ModelResource>>> GetAllAsync(int? makeId, int page = 1, int pageSize = 5); 
        Task<ServiceResult<ModelResource>> GetByIdAsync(int id);
        Task<ServiceResult<ModelResource>> CreateAsync(SaveModelResource modelResource);
        Task<ServiceResult<ModelResource>> UpdateAsync(int id, UpdateModelResource modelResource);
        Task<ServiceResult<ModelResource>> DeleteAsync(int id);

    }
}
