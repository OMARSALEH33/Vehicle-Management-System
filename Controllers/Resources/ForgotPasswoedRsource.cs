using System.ComponentModel.DataAnnotations;

namespace Challenge_1_Build_APIS.Controllers.Resources
{
    public class ForgotPasswordRsource
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string ClientUri { get; set; } = string.Empty;

    }
}
