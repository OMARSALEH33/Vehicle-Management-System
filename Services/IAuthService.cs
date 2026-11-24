using Challenge_1_Build_APIS.Controllers.Resources;

namespace Challenge_1_Build_APIS.Services
{
    public interface IAuthService
    {
        Task<ServiceResult<string>> LoginAsync(LoginResource resource);
        Task<ServiceResult<string>> ForgotPasswordAsync(ForgotPasswordRsource resource);
        Task<ServiceResult<bool>> ResetPasswordAsync(ResetPasswordResources resource);

    }
}
