using System.ComponentModel.DataAnnotations;

namespace Challenge_1_Build_APIS.Controllers.Resources
{
    public class ResetPasswordResources
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
      

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

    }
}
