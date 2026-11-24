using Challenge_1_Build_APIS.Controllers.Resources;

namespace Challenge_1_Build_APIS.Services
{
    public interface IVehicleService
    {
        Task<ServiceResult<PagedResult<VehicleResource>>> GetAllAsync(int page = 1, int pageSize = 5);

        Task<ServiceResult<VehicleResource>> GetByIdAsync(int id);

        Task<ServiceResult<VehicleResource>> CreateAsync(SaveVehicleResource vehicleResource);

        Task<ServiceResult<VehicleResource>> UpdateAsync(int id, SaveVehicleResource vehicleResource);

        Task<ServiceResult<VehicleResource>> DeleteAsync(int id);
    }
}