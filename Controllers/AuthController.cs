using Challenge_1_Build_APIS.Controllers.Resources;
using Challenge_1_Build_APIS.Services;
using Microsoft.AspNetCore.Mvc;

namespace Challenge_1_Build_APIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginResource resource)
        {
            var result = await authService.LoginAsync(resource);

            if (!result.IsSuccess)
                return Unauthorized(new { message = "Invalid username or password" });

            return Ok(new { Token = result.Data });
        }
        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRsource resource)
        {
            var result = await authService.ForgotPasswordAsync(resource);

            if (!result.IsSuccess)
                return NotFound(new { message = "User not found or invalid email." });

            return Ok(new { message = "Password reset link generated.", resetLink = result.Data });
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordResources resource)
        {
            var result = await authService.ResetPasswordAsync(resource);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(result.Field!, result.Error!);
                return ValidationProblem(ModelState);
            }

            return Ok(new { message = "Password has been reset successfully." });
        }


    }
}