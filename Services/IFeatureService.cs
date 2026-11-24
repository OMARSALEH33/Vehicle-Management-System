using Challenge_1_Build_APIS.Controllers.Resources;

namespace Challenge_1_Build_APIS.Services
{
    public interface IFeatureService
    {
        Task<IEnumerable<FeatureResource>> GetAllAsync();
        Task<ServiceResult<FeatureResource>> GetByIdAsync(int id);
        Task<ServiceResult<FeatureResource>> CreateAsync(SaveFeatureResource featureResource);
        Task<ServiceResult<FeatureResource>> UpdateAsync(int id, SaveFeatureResource featureResource);
        Task<ServiceResult<FeatureResource>> DeleteAsync(int id);
    }
}
